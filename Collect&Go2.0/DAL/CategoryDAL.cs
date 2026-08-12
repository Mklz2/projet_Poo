using Collect_Go2._0.Interfaces;
using Collect_Go2._0.Models;
using Microsoft.Data.SqlClient;

namespace Collect_Go2._0.DAL
{
    public class CategoryDAL : ICategoryDAL
    {
        private readonly string _connectionString;

        public CategoryDAL(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ClickCollect")!;
        }

        public async Task<List<Category>> GetAllAsync()
        {
            List<Category> categoryList = new List<Category>();

            string query = "SELECT * FROM Category";

            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);

            await connection.OpenAsync();

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                categoryList.Add(ReadCategory(reader));
            }

            return categoryList;
        }

        public async Task<Category?> GetByIdAsync(int categoryId)
        {
            string query = "SELECT * FROM Category WHERE CategoryId = @CategoryId";

            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@CategoryId", categoryId);

            await connection.OpenAsync();

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return ReadCategory(reader);
        }

        private static Category ReadCategory(SqlDataReader reader)
        {
            return new Category(
                Convert.ToInt32(reader["CategoryId"]),
                reader["Name"].ToString()!);
        }
    }
}
