using Collect_Go2._0.Models;
using Microsoft.Data.SqlClient;
namespace Collect_Go2._0.DAL
{

    public class UserRepository
    {
        private SqlConnection _connection;

        public UserRepository()
        {
            string connectionString =
                "Server=.\\SQLEXPRESS;Database=ClickAndCollect;Trusted_Connection=True;TrustServerCertificate=True;";

            _connection = new SqlConnection(connectionString);
        }

        //Create Account Function

        public bool AddUser(User user)
        {
            string query = @"INSERT INTO Users
                            (FirstName, LastName, Email, Password, Role, StoreId)
                            VALUES
                            (@FirstName, @LastName, @Email, @Password, @Role, @StoreId)";

            using (SqlCommand command = new SqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@FirstName", user.FirstName);
                command.Parameters.AddWithValue("@LastName", user.LastName);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@Password", user.Password);
                command.Parameters.AddWithValue("@Role", user.Role);

                if (user.StoreId.HasValue)
                    command.Parameters.AddWithValue("@StoreId", user.StoreId.Value);
                else
                    command.Parameters.AddWithValue("@StoreId", DBNull.Value);

                _connection.Open();

                int rows = command.ExecuteNonQuery();
                Console.WriteLine($"Lignes insérées : {rows}");
                _connection.Close();

                return rows > 0;
            }
        }


        //Login Function

        public User GetUserByEmailAndPassword(string email, string password)
        {
            User user = null;

            string query = @"SELECT * FROM Users
                     WHERE Email = @Email
                     AND Password = @Password";

            using (SqlCommand command = new SqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Password", password);

                _connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new User();

                        user.Id = Convert.ToInt32(reader["Id"]);
                        user.FirstName = reader["FirstName"].ToString();
                        user.LastName = reader["LastName"].ToString();
                        user.Email = reader["Email"].ToString();
                        user.Password = reader["Password"].ToString();
                        user.Role = reader["Role"].ToString();

                        if (reader["StoreId"] != DBNull.Value)
                        {
                            user.StoreId = Convert.ToInt32(reader["StoreId"]);
                        }
                    }
                }

                _connection.Close();
            }

            return user;
        }




    }
}

