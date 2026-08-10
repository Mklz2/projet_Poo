using Collect_Go2._0.Models;
using Microsoft.Data.SqlClient;
using static System.Formats.Asn1.AsnWriter;

namespace Collect_Go2._0.DAL
{
    public class StoresRepository
    {
        private SqlConnection _connection;

        public StoresRepository()
        {
            string connectionString =
                "Server=.\\SQLEXPRESS;Database=ClickAndCollect;Trusted_Connection=True;TrustServerCertificate=True;";

            _connection = new SqlConnection(connectionString);
        }

        public List<Stores> GetAllStores()
        {
            List<Stores> storeList = new List<Stores>();

            string query = "SELECT * FROM Stores";

            using (SqlCommand command = new SqlCommand(query, _connection))
            {
                _connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Stores store = new Stores();

                        store.Id = Convert.ToInt32(reader["StoreId"]);
                        store.Name = reader["Name"].ToString();
                        store.Address = reader["Address"].ToString();

                        storeList.Add(store);
                    }
                }

                _connection.Close();
            }

            return storeList;
        }

        public Stores GetStoreById(int id)
        {
            Stores store = null;

            string query = "SELECT * FROM Stores WHERE Id = @Id";

            using (SqlCommand command = new SqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@Id", id);

                _connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        store = new Stores();

                        store.Id = Convert.ToInt32(reader["StoreId"]);
                        store.Name = reader["Name"].ToString();
                        store.Address = reader["Address"].ToString();
                    }
                }

                _connection.Close();
            }

            return store;
        }
    }
}