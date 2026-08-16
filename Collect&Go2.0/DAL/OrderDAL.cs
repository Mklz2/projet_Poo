using Collect_Go2._0.Interfaces;
using Collect_Go2._0.Models;
using Microsoft.Data.SqlClient;

namespace Collect_Go2._0.DAL
{
    public class OrderDAL : IOrderDAL
    {
        private readonly string _connectionString;

        public OrderDAL(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ClickCollect")!;
        }

        public async Task CreateAsync(Order order)
        {
            if (order.Client == null || order.Store == null || order.TimeSlot == null)
                throw new InvalidOperationException("La commande doit avoir un client, un magasin et un créneau.");

            if (order.OrderItems.Count == 0)
                throw new InvalidOperationException("La commande ne peut pas être vide.");

            string insertOrder = @"INSERT INTO [Order]
                                        (ClientId, StoreId, TimeSlotId, OrderDate, NumberOfBoxes, ReturnedBoxes, TotalAmount, Status)
                                    OUTPUT INSERTED.OrderId
                                    VALUES
                                        (@ClientId, @StoreId, @TimeSlotId, @OrderDate, @NumberOfBoxes, @ReturnedBoxes, @TotalAmount, @Status)";

            string insertItem = @"INSERT INTO OrderItem (OrderId, ProductId, Quantity)
                                   VALUES (@OrderId, @ProductId, @Quantity)";

            string reserveSlot = @"UPDATE TimeSlot
                                    SET ReservationCount = ReservationCount + 1
                                    WHERE TimeSlotId = @TimeSlotId AND ReservationCount < @MaxReservation";

            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using SqlTransaction transaction = connection.BeginTransaction();

            try
            {
                using (SqlCommand command = new SqlCommand(reserveSlot, connection, transaction))
                {
                    command.Parameters.AddWithValue("@TimeSlotId", order.TimeSlot.TimeSlotId);
                    command.Parameters.AddWithValue("@MaxReservation", TimeSlot.MaxReservation);

                    int rows = await command.ExecuteNonQueryAsync();

                    if (rows == 0)
                        throw new InvalidOperationException("Ce créneau vient d'être complété, veuillez en choisir un autre.");
                }

                int orderId;
                using (SqlCommand command = new SqlCommand(insertOrder, connection, transaction))
                {
                    command.Parameters.AddWithValue("@ClientId", order.Client.UserId);
                    command.Parameters.AddWithValue("@StoreId", order.Store.StoreId);
                    command.Parameters.AddWithValue("@TimeSlotId", order.TimeSlot.TimeSlotId);
                    command.Parameters.AddWithValue("@OrderDate", DateTime.Now);
                    command.Parameters.AddWithValue("@NumberOfBoxes", (object?)order.NumberOfBoxes ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ReturnedBoxes", (object?)order.ReturnedBoxes ?? DBNull.Value);
                    command.Parameters.AddWithValue("@TotalAmount", DBNull.Value);
                    command.Parameters.AddWithValue("@Status", order.Status.ToString());

                    orderId = (int)(await command.ExecuteScalarAsync())!;
                }

                foreach (OrderItem item in order.OrderItems)
                {
                    using SqlCommand command = new SqlCommand(insertItem, connection, transaction);
                    command.Parameters.AddWithValue("@OrderId", orderId);
                    command.Parameters.AddWithValue("@ProductId", item.Product!.ProductId);
                    command.Parameters.AddWithValue("@Quantity", item.Quantity);

                    await command.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Order?> GetByIdAsync(int orderId)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            Order? order = await ReadOrderHeaderAsync(connection, "WHERE o.OrderId = @Id", "@Id", orderId);

            if (order != null)
                order.OrderItems = await ReadOrderItemsAsync(connection, order.OrderId);

            return order;
        }

        public async Task<List<Order>> GetByClientAsync(int clientId)
        {
            List<Order> orders = new List<Order>();

            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string query = BuildHeaderQuery("WHERE o.ClientId = @ClientId ORDER BY o.OrderDate DESC");

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ClientId", clientId);

                using SqlDataReader reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    orders.Add(ReadOrder(reader));
                }
            }

            foreach (Order order in orders)
            {
                order.OrderItems = await ReadOrderItemsAsync(connection, order.OrderId);
            }

            return orders;
        }

        public async Task UpdateStatusAsync(int orderId, OrderStatus status)
        {
            string query = "UPDATE [Order] SET Status = @Status WHERE OrderId = @OrderId";

            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Status", status.ToString());
            command.Parameters.AddWithValue("@OrderId", orderId);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task<List<Order>> GetByStoreAndStatusAsync(int storeId, OrderStatus status, DateTime date)
        {
            List<Order> orders = new List<Order>();

            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string query = BuildHeaderQuery(
                "WHERE o.StoreId = @StoreId AND o.Status = @Status AND t.Date = @Date ORDER BY t.StartHour");

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@StoreId", storeId);
                command.Parameters.AddWithValue("@Status", status.ToString());
                command.Parameters.AddWithValue("@Date", date.Date);

                using SqlDataReader reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    orders.Add(ReadOrder(reader));
                }
            }

            foreach (Order order in orders)
            {
                order.OrderItems = await ReadOrderItemsAsync(connection, order.OrderId);
            }

            return orders;
        }

        public async Task MarkPreparedAsync(int orderId, int numberOfBoxes)
        {
            string query = @"UPDATE [Order]
                              SET NumberOfBoxes = @NumberOfBoxes, Status = @NewStatus
                              WHERE OrderId = @OrderId AND Status = @ExpectedStatus";

            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@NumberOfBoxes", numberOfBoxes);
            command.Parameters.AddWithValue("@NewStatus", OrderStatus.Prepared.ToString());
            command.Parameters.AddWithValue("@OrderId", orderId);
            command.Parameters.AddWithValue("@ExpectedStatus", OrderStatus.Placed.ToString());

            await connection.OpenAsync();

            int rows = await command.ExecuteNonQueryAsync();

            if (rows == 0)
                throw new InvalidOperationException("Cette commande n'est plus au statut attendu (déjà préparée ?).");
        }

        public async Task ApproveOrderAsync(int orderId, int returnedBoxes, double totalAmount)
        {
            string query = @"UPDATE [Order]
                              SET ReturnedBoxes = @ReturnedBoxes, TotalAmount = @TotalAmount, Status = @NewStatus
                              WHERE OrderId = @OrderId AND Status = @ExpectedStatus";

            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ReturnedBoxes", returnedBoxes);
            command.Parameters.AddWithValue("@TotalAmount", totalAmount);
            command.Parameters.AddWithValue("@NewStatus", OrderStatus.Honored.ToString());
            command.Parameters.AddWithValue("@OrderId", orderId);
            command.Parameters.AddWithValue("@ExpectedStatus", OrderStatus.Prepared.ToString());

            await connection.OpenAsync();

            int rows = await command.ExecuteNonQueryAsync();

            if (rows == 0)
                throw new InvalidOperationException("Cette commande n'est plus au statut attendu (déjà honorée ?).");
        }

        private static string BuildHeaderQuery(string whereClause)
        {
            return $@"SELECT o.OrderId, o.OrderDate, o.NumberOfBoxes, o.ReturnedBoxes, o.TotalAmount, o.Status,
                             u.UserId AS ClientUserId, u.Firstname AS ClientFirstname, u.Lastname AS ClientLastname,
                             u.Email AS ClientEmail, u.Password AS ClientPassword, cl.Phone AS ClientPhone,
                             s.StoreId, s.Name AS StoreName, s.Address AS StoreAddress, s.City AS StoreCity,
                             t.TimeSlotId, t.Date AS SlotDate, t.StartHour, t.EndHour, t.ReservationCount
                      FROM [Order] o
                      JOIN Client cl ON cl.UserId = o.ClientId
                      JOIN Users u ON u.UserId = cl.UserId
                      JOIN Store s ON s.StoreId = o.StoreId
                      JOIN TimeSlot t ON t.TimeSlotId = o.TimeSlotId
                      {whereClause}";
        }

        private static async Task<Order?> ReadOrderHeaderAsync(SqlConnection connection, string whereClause, string paramName, int paramValue)
        {
            string query = BuildHeaderQuery(whereClause);

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue(paramName, paramValue);

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return ReadOrder(reader);
        }

        private static Order ReadOrder(SqlDataReader reader)
        {
            Client client = new Client(
                Convert.ToInt32(reader["ClientUserId"]),
                reader["ClientFirstname"].ToString()!,
                reader["ClientLastname"].ToString()!,
                reader["ClientEmail"].ToString()!,
                reader["ClientPassword"].ToString()!,
                reader["ClientPhone"].ToString()!);

            Store store = new Store(
                Convert.ToInt32(reader["StoreId"]),
                reader["StoreName"].ToString()!,
                reader["StoreAddress"].ToString()!,
                reader["StoreCity"].ToString()!);

            TimeSlot slot = new TimeSlot(
                Convert.ToInt32(reader["TimeSlotId"]),
                Convert.ToDateTime(reader["SlotDate"]),
                DateTime.Today.Add((TimeSpan)reader["StartHour"]),
                DateTime.Today.Add((TimeSpan)reader["EndHour"]));

            Order order = new Order(
                Convert.ToInt32(reader["OrderId"]),
                Convert.ToDateTime(reader["OrderDate"]),
                client,
                store,
                slot)
            {
                Status = Enum.Parse<OrderStatus>(reader["Status"].ToString()!)
            };

            if (reader["NumberOfBoxes"] != DBNull.Value)
                order.NumberOfBoxes = Convert.ToInt32(reader["NumberOfBoxes"]);

            if (reader["ReturnedBoxes"] != DBNull.Value)
                order.ReturnedBoxes = Convert.ToInt32(reader["ReturnedBoxes"]);

            if (reader["TotalAmount"] != DBNull.Value)
                order.SetPersistedTotal(Convert.ToDouble(reader["TotalAmount"]));

            return order;
        }

        private static async Task<List<OrderItem>> ReadOrderItemsAsync(SqlConnection connection, int orderId)
        {
            List<OrderItem> items = new List<OrderItem>();

            string query = @"SELECT oi.OrderItemId, oi.Quantity,
                                     p.ProductId, p.Name, p.Price, p.Description, p.ImageUrl,
                                     c.CategoryId, c.Name AS CategoryName
                              FROM OrderItem oi
                              JOIN Product p ON p.ProductId = oi.ProductId
                              JOIN Category c ON c.CategoryId = p.CategoryId
                              WHERE oi.OrderId = @OrderId";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@OrderId", orderId);

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                Category category = new Category(
                    Convert.ToInt32(reader["CategoryId"]),
                    reader["CategoryName"].ToString()!);

                Product product = new Product(
                    Convert.ToInt32(reader["ProductId"]),
                    reader["Name"].ToString()!,
                    Convert.ToDouble(reader["Price"]),
                    reader["Description"].ToString()!,
                    reader["ImageUrl"].ToString()!,
                    category);

                items.Add(new OrderItem(
                    Convert.ToInt32(reader["OrderItemId"]),
                    product,
                    Convert.ToInt32(reader["Quantity"])));
            }

            return items;
        }
    }
}
