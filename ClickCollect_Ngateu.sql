-- ============================================================
-- Click & Collect - ClickCollect_Ngateu
-- Script unique : création de la base + schéma (tables/contraintes)
-- Compatible SQL Server (ADO.NET brut, pas d'ORM)
-- ============================================================

IF DB_ID('ClickCollect_Ngateu') IS NOT NULL
BEGIN
    ALTER DATABASE ClickCollect_Ngateu SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE ClickCollect_Ngateu;
END
GO

CREATE DATABASE ClickCollect_Ngateu;
GO

USE ClickCollect_Ngateu;
GO

-- ============================================================
-- Schéma
-- ============================================================

CREATE TABLE Users (
    UserId    INT IDENTITY(1,1) PRIMARY KEY,
    Firstname NVARCHAR(50)  NOT NULL,
    Lastname  NVARCHAR(50)  NOT NULL,
    Email     NVARCHAR(100) NOT NULL UNIQUE,
    Password  NVARCHAR(200) NOT NULL  -- hash BCrypt
);
GO

CREATE TABLE Store (
    StoreId INT IDENTITY(1,1) PRIMARY KEY,
    Name    NVARCHAR(100) NOT NULL,
    Address NVARCHAR(150) NOT NULL,
    City    NVARCHAR(50)  NOT NULL
);
GO

CREATE TABLE Client (
    UserId INT PRIMARY KEY REFERENCES Users(UserId) ON DELETE CASCADE,
    Phone  NVARCHAR(20) NOT NULL
);
GO

CREATE TABLE Cashier (
    UserId     INT PRIMARY KEY REFERENCES Users(UserId) ON DELETE CASCADE,
    StoreId    INT NOT NULL REFERENCES Store(StoreId),
    HiringDate DATE NOT NULL
);
GO

CREATE TABLE OrderPicker (
    UserId     INT PRIMARY KEY REFERENCES Users(UserId) ON DELETE CASCADE,
    StoreId    INT NOT NULL REFERENCES Store(StoreId),
    HiringDate DATE NOT NULL
);
GO

CREATE TABLE Category (
    CategoryId  INT IDENTITY(1,1) PRIMARY KEY,
    Name        NVARCHAR(50) NOT NULL,
    Description NVARCHAR(200) NULL
);
GO

CREATE TABLE Product (
    ProductId   INT IDENTITY(1,1) PRIMARY KEY,
    Name        NVARCHAR(100) NOT NULL,
    Price       DECIMAL(10,2) NOT NULL CHECK (Price >= 0),
    Description NVARCHAR(300) NULL,
    ImageUrl    NVARCHAR(300) NULL,
    CategoryId  INT NOT NULL REFERENCES Category(CategoryId)
);
GO

CREATE TABLE TimeSlot (
    TimeSlotId       INT IDENTITY(1,1) PRIMARY KEY,
    StoreId          INT NOT NULL REFERENCES Store(StoreId),
    Date             DATE NOT NULL,
    StartHour        TIME NOT NULL,
    EndHour          TIME NOT NULL,
    ReservationCount INT NOT NULL DEFAULT 0 CHECK (ReservationCount BETWEEN 0 AND 10),
    CHECK (EndHour > StartHour)
);
GO

CREATE TABLE [Order] (
    OrderId        INT IDENTITY(1,1) PRIMARY KEY,
    ClientId       INT NOT NULL REFERENCES Client(UserId),
    StoreId        INT NOT NULL REFERENCES Store(StoreId),
    TimeSlotId     INT NOT NULL REFERENCES TimeSlot(TimeSlotId),
    OrderDate      DATETIME NOT NULL DEFAULT GETDATE(),
    NumberOfBoxes  INT NULL,              -- rempli par le préparateur
    ReturnedBoxes  INT NULL,              -- rempli par le caissier
    TotalAmount    DECIMAL(10,2) NULL,    -- calculé au paiement
    Status         NVARCHAR(20) NOT NULL DEFAULT 'Placed'
                   CHECK (Status IN ('Placed','Prepared','Honored'))
);
GO

CREATE TABLE OrderItem (
    OrderItemId INT IDENTITY(1,1) PRIMARY KEY,
    OrderId     INT NOT NULL REFERENCES [Order](OrderId) ON DELETE CASCADE,
    ProductId   INT NOT NULL REFERENCES Product(ProductId),
    Quantity    INT NOT NULL CHECK (Quantity > 0),
    CONSTRAINT UQ_OrderItem_Order_Product UNIQUE (OrderId, ProductId)
);
GO

-- ============================================================
-- Données de test
-- ============================================================

INSERT INTO Store (Name, Address, City) VALUES
    (N'ClickGo Charleroi', N'Rue de la Gare 1', N'Charleroi'),
    (N'ClickGo Mons', N'Grand Place 5', N'Mons');
GO

INSERT INTO Category (Name, Description) VALUES
    (N'Fruits et legumes', N'Produits frais de saison'),
    (N'Produits laitiers', N'Lait, fromages, yaourts'),
    (N'Epicerie', N'Produits secs et conserves'),
    (N'Surgeles', N'Produits congeles');
GO

INSERT INTO Product (Name, Price, Description, ImageUrl, CategoryId) VALUES
    (N'Pommes', 2.50, N'Pommes Golden 1kg', '/images/pommes.jpg', (SELECT CategoryId FROM Category WHERE Name = N'Fruits et legumes')),
    (N'Bananes', 1.80, N'Bananes bio 1kg', '/images/bananes.jpg', (SELECT CategoryId FROM Category WHERE Name = N'Fruits et legumes')),
    (N'Carottes', 1.20, N'Carottes 1kg', '/images/carottes.jpg', (SELECT CategoryId FROM Category WHERE Name = N'Fruits et legumes')),
    (N'Lait demi-ecreme', 1.10, N'Lait 1L', '/images/lait.jpg', (SELECT CategoryId FROM Category WHERE Name = N'Produits laitiers')),
    (N'Fromage Gouda', 3.40, N'Gouda 300g', '/images/gouda.jpg', (SELECT CategoryId FROM Category WHERE Name = N'Produits laitiers')),
    (N'Yaourts nature', 2.10, N'Pack de 8', '/images/yaourts.jpg', (SELECT CategoryId FROM Category WHERE Name = N'Produits laitiers')),
    (N'Riz basmati', 2.80, N'Sac 1kg', '/images/riz.jpg', (SELECT CategoryId FROM Category WHERE Name = N'Epicerie')),
    (N'Pates penne', 1.50, N'Paquet 500g', '/images/pates.jpg', (SELECT CategoryId FROM Category WHERE Name = N'Epicerie')),
    (N'Pizza surgelee', 3.90, N'Pizza margherita', '/images/pizza.jpg', (SELECT CategoryId FROM Category WHERE Name = N'Surgeles')),
    (N'Frites surgelees', 2.30, N'Sachet 1kg', '/images/frites.jpg', (SELECT CategoryId FROM Category WHERE Name = N'Surgeles'));
GO

-- Employés : encodés directement en BDD (pas d'interface admin, conforme à l'énoncé)
-- Mot de passe de test, EN CLAIR pour l'instant (BCrypt pas encore branché) : Test1234!
INSERT INTO Users (Firstname, Lastname, Email, Password) VALUES
    (N'Alice', N'Caissiere', N'alice.cashier@clickgo.test', N'Test1234!'),
    (N'Bruno', N'Preparateur', N'bruno.picker@clickgo.test', N'Test1234!'),
    (N'Chloe', N'Caissiere', N'chloe.cashier@clickgo.test', N'Test1234!'),
    (N'David', N'Preparateur', N'david.picker@clickgo.test', N'Test1234!');
GO

INSERT INTO Cashier (UserId, StoreId, HiringDate) VALUES
    ((SELECT UserId FROM Users WHERE Email = N'alice.cashier@clickgo.test'), (SELECT StoreId FROM Store WHERE Name = N'ClickGo Charleroi'), '2025-01-15'),
    ((SELECT UserId FROM Users WHERE Email = N'chloe.cashier@clickgo.test'), (SELECT StoreId FROM Store WHERE Name = N'ClickGo Mons'), '2025-03-01');
GO

INSERT INTO OrderPicker (UserId, StoreId, HiringDate) VALUES
    ((SELECT UserId FROM Users WHERE Email = N'bruno.picker@clickgo.test'), (SELECT StoreId FROM Store WHERE Name = N'ClickGo Charleroi'), '2025-02-01'),
    ((SELECT UserId FROM Users WHERE Email = N'david.picker@clickgo.test'), (SELECT StoreId FROM Store WHERE Name = N'ClickGo Mons'), '2025-04-01');
GO

-- Créneaux horaires : 7 jours à partir de demain (règle "pas de réservation le jour même"), 9h-18h, tranches d'1h
DECLARE @StoreId INT;
DECLARE @Day INT;
DECLARE @Hour INT;
DECLARE @StartDate DATE = DATEADD(DAY, 1, CAST(GETDATE() AS DATE));

DECLARE store_cursor CURSOR FOR SELECT StoreId FROM Store;
OPEN store_cursor;
FETCH NEXT FROM store_cursor INTO @StoreId;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @Day = 0;
    WHILE @Day < 7
    BEGIN
        SET @Hour = 9;
        WHILE @Hour < 18
        BEGIN
            INSERT INTO TimeSlot (StoreId, Date, StartHour, EndHour, ReservationCount)
            VALUES (
                @StoreId,
                DATEADD(DAY, @Day, @StartDate),
                CAST(RIGHT('0' + CAST(@Hour AS VARCHAR), 2) + ':00' AS TIME),
                CAST(RIGHT('0' + CAST(@Hour + 1 AS VARCHAR), 2) + ':00' AS TIME),
                0
            );
            SET @Hour = @Hour + 1;
        END
        SET @Day = @Day + 1;
    END
    FETCH NEXT FROM store_cursor INTO @StoreId;
END

CLOSE store_cursor;
DEALLOCATE store_cursor;
GO
