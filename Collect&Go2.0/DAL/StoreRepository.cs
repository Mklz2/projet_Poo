using Collect_Go2._0.Models;
using Microsoft.Data.SqlClient;

namespace Collect_Go2._0.DAL
{
    public class StoreRepository
    {
        private SqlConnection _connection;

        public StoreRepository()
        {
            string connectionString =
                "Server=.\\SQLEXPRESS;Database=ClickAndCollect;Trusted_Connection=True;TrustServerCertificate=True;";

            _connection = new SqlConnection(connectionString);
        }

        public List<Store> GetAllStores()
        {
            List<Store> storeList = new List<Store>();

            string query = "SELECT * FROM Stores";

            using (SqlCommand command = new SqlCommand(query, _connection))
            {
                _connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Store store = new Store();

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
    }
}