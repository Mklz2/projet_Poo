using Collect_Go2._0.Interfaces;
using Collect_Go2._0.Models;
using Microsoft.Data.SqlClient;

namespace Collect_Go2._0.DAL
{
    public class ProductDAL : IProductDAL
    {
        private readonly string _connectionString;

        public ProductDAL(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ClickCollect")!;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            List<Product> productList = new List<Product>();

            string query = @"SELECT p.ProductId, p.Name, p.Price,
                                    p.Description, p.ImageUrl,
                                    c.CategoryId,
                                    c.Name AS CategoryName
                             FROM Product p
                             INNER JOIN Category c
                             ON p.CategoryId = c.CategoryId";

            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);

            await connection.OpenAsync();

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                productList.Add(ReadProduct(reader));
            }

            return productList;
        }

        public async Task<List<Product>> GetByCategoryAsync(int categoryId)
        {
            List<Product> productList = new List<Product>();

            string query = @"SELECT p.ProductId, p.Name, p.Price,
                                    p.Description, p.ImageUrl,
                                    c.CategoryId,
                                    c.Name AS CategoryName
                             FROM Product p
                             INNER JOIN Category c
                             ON p.CategoryId = c.CategoryId
                             WHERE p.CategoryId = @CategoryId";

            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@CategoryId", categoryId);

            await connection.OpenAsync();

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                productList.Add(ReadProduct(reader));
            }

            return productList;
        }

        public async Task<Product?> GetByIdAsync(int productId)
        {
            string query = @"SELECT p.ProductId, p.Name, p.Price,
                                    p.Description, p.ImageUrl,
                                    c.CategoryId,
                                    c.Name AS CategoryName
                             FROM Product p
                             INNER JOIN Category c
                             ON p.CategoryId = c.CategoryId
                             WHERE p.ProductId = @ProductId";

            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ProductId", productId);

            await connection.OpenAsync();

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return ReadProduct(reader);
        }

        private static Product ReadProduct(SqlDataReader reader)
        {
            Category category = new Category(
                Convert.ToInt32(reader["CategoryId"]),
                reader["CategoryName"].ToString()!);

            return new Product(
                Convert.ToInt32(reader["ProductId"]),
                reader["Name"].ToString()!,
                Convert.ToDouble(reader["Price"]),
                reader["Description"].ToString()!,
                reader["ImageUrl"].ToString()!,
                category);
        }
    }
}
