using Collect_Go2._0.Interfaces;
using Collect_Go2._0.Models;
using Microsoft.Data.SqlClient;

namespace Collect_Go2._0.DAL
{
    public class StoreDAL : IStoreDAL
    {
        private readonly string _connectionString;

        public StoreDAL(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ClickCollect")!;
        }

        public async Task<List<Store>> GetAllAsync()
        {
            List<Store> storeList = new List<Store>();

            string query = "SELECT * FROM Store";

            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);

            await connection.OpenAsync();

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                storeList.Add(ReadStore(reader));
            }

            return storeList;
        }

        public async Task<Store?> GetByIdAsync(int storeId)
        {
            string query = "SELECT * FROM Store WHERE StoreId = @StoreId";

            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@StoreId", storeId);

            await connection.OpenAsync();

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return ReadStore(reader);
        }

        private static Store ReadStore(SqlDataReader reader)
        {
            return new Store(
                Convert.ToInt32(reader["StoreId"]),
                reader["Name"].ToString()!,
                reader["Address"].ToString()!,
                reader["City"].ToString()!);
        }
    }
}
