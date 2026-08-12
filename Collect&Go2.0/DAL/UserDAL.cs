using Collect_Go2._0.Interfaces;
using Collect_Go2._0.Models;
using Microsoft.Data.SqlClient;

namespace Collect_Go2._0.DAL
{
    public class UserDAL : IUserDAL
    {
        private readonly string _connectionString;

        public UserDAL(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ClickCollect")!;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            string query = "SELECT COUNT(1) FROM Users WHERE Email = @Email";

            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Email", email);

            await connection.OpenAsync();

            int count = (int)(await command.ExecuteScalarAsync())!;
            return count > 0;
        }

        // Inscription d'un client (le préparateur/caissier sont encodés directement en BDD, pas via l'appli)
        public async Task CreateAsync(Client client)
        {
            string insertUser = @"INSERT INTO Users (Firstname, Lastname, Email, Password)
                                   OUTPUT INSERTED.UserId
                                   VALUES (@Firstname, @Lastname, @Email, @Password)";

            string insertClient = @"INSERT INTO Client (UserId, Phone) VALUES (@UserId, @Phone)";

            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using SqlTransaction transaction = connection.BeginTransaction();

            try
            {
                int userId;
                using (SqlCommand command = new SqlCommand(insertUser, connection, transaction))
                {
                    command.Parameters.AddWithValue("@Firstname", client.Firstname);
                    command.Parameters.AddWithValue("@Lastname", client.Lastname);
                    command.Parameters.AddWithValue("@Email", client.Email);
                    command.Parameters.AddWithValue("@Password", client.Password);

                    userId = (int)(await command.ExecuteScalarAsync())!;
                }

                using (SqlCommand command = new SqlCommand(insertClient, connection, transaction))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@Phone", client.Phone);

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

        // Connexion : identifie le bon sous-type (Client / Cashier / OrderPicker) selon la table où l'UserId existe
        public async Task<User?> GetByEmailAndPasswordAsync(string email, string password)
        {
            string query = @"
                SELECT u.UserId, u.Firstname, u.Lastname, u.Email, u.Password,
                       cl.Phone,
                       ca.StoreId AS CashierStoreId, ca.HiringDate AS CashierHiringDate,
                       op.StoreId AS PickerStoreId, op.HiringDate AS PickerHiringDate,
                       s.StoreId AS EmployeeStoreId, s.Name AS StoreName,
                       s.Address AS StoreAddress, s.City AS StoreCity
                FROM Users u
                LEFT JOIN Client cl ON cl.UserId = u.UserId
                LEFT JOIN Cashier ca ON ca.UserId = u.UserId
                LEFT JOIN OrderPicker op ON op.UserId = u.UserId
                LEFT JOIN Store s ON s.StoreId = ca.StoreId OR s.StoreId = op.StoreId
                WHERE u.Email = @Email AND u.Password = @Password";

            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Email", email);
            command.Parameters.AddWithValue("@Password", password);

            await connection.OpenAsync();

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            int userId = Convert.ToInt32(reader["UserId"]);
            string firstname = reader["Firstname"].ToString()!;
            string lastname = reader["Lastname"].ToString()!;
            string userEmail = reader["Email"].ToString()!;
            string userPassword = reader["Password"].ToString()!;

            if (reader["Phone"] != DBNull.Value)
            {
                string phone = reader["Phone"].ToString()!;
                return new Client(userId, firstname, lastname, userEmail, userPassword, phone);
            }

            if (reader["EmployeeStoreId"] != DBNull.Value)
            {
                Store store = new Store(
                    Convert.ToInt32(reader["EmployeeStoreId"]),
                    reader["StoreName"].ToString()!,
                    reader["StoreAddress"].ToString()!,
                    reader["StoreCity"].ToString()!);

                if (reader["CashierStoreId"] != DBNull.Value)
                {
                    DateTime hiringDate = Convert.ToDateTime(reader["CashierHiringDate"]);
                    return new Cashier(userId, firstname, lastname, userEmail, userPassword, store, hiringDate);
                }

                if (reader["PickerStoreId"] != DBNull.Value)
                {
                    DateTime hiringDate = Convert.ToDateTime(reader["PickerHiringDate"]);
                    return new OrderPicker(userId, firstname, lastname, userEmail, userPassword, store, hiringDate);
                }
            }

            return null;
        }
    }
}
