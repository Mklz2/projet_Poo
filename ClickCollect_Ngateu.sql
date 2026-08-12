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
-- Données de test : à compléter à l'étape suivante
-- (magasins, catégories, produits, créneaux, employés avec hash BCrypt réel)
-- ============================================================
