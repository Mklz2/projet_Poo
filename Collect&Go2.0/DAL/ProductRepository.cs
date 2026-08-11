
using Collect_Go2._0.Models;
using Microsoft.Data.SqlClient;

namespace Collect_Go2._0.DAL
{
    public class ProductRepository
    {
        private SqlConnection _connection;

        public ProductRepository()
        {
            string connectionString =
                "Server=.\\SQLEXPRESS;Database=ClickAndCollect;Trusted_Connection=True;TrustServerCertificate=True;";

            _connection = new SqlConnection(connectionString);
        }

        public List<Product> GetAllProduct()
        {
            List<Product> productList = new List<Product>();

            string query = @"SELECT p.ProductId, p.Name, p.Price,
                                    p.Description, p.ImageUrl,
                                    c.CategoryId,
                                    c.Name AS CategoryName
                             FROM Product p
                             INNER JOIN Categories c
                             ON p.CategoryId = c.CategoryId";

            using (SqlCommand command = new SqlCommand(query, _connection))
            {
                _connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Product product = new Product();

                        product.ProductId = Convert.ToInt32(reader["ProductId"]);
                        product.Name = reader["Name"].ToString();
                        product.Price = Convert.ToDecimal(reader["Price"]);
                        product.Description = reader["Description"].ToString();
                        product.ImageUrl = reader["ImageUrl"].ToString();
                        product.CategoryId = Convert.ToInt32(reader["CategoryId"]);

                        product.Category = new Category();
                        product.Category.CategoryId = product.CategoryId;
                        product.Category.Name = reader["CategoryName"].ToString();

                        productList.Add(product);
                    }
                }

                _connection.Close();
            }

            return productList;
        }

        public List<Product> GetProductByCategory(int categoryId)
        {
            List<Product> productList = new List<Product>();

            string query = @"SELECT p.ProductId, p.Name, p.Price,
                                    p.Description, p.ImageUrl,
                                    c.CategoryId,
                                    c.Name AS CategoryName
                             FROM Product p
                             INNER JOIN Categories c
                             ON p.CategoryId = c.CategoryId
                             WHERE p.CategoryId = @CategoryId";

            using (SqlCommand command = new SqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@CategoryId", categoryId);

                _connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Product product = new Product();

                        product.ProductId = Convert.ToInt32(reader["ProductId"]);
                        product.Name = reader["Name"].ToString();
                        product.Price = Convert.ToDecimal(reader["Price"]);
                        product.Description = reader["Description"].ToString();
                        product.ImageUrl = reader["ImageUrl"].ToString();
                        product.CategoryId = Convert.ToInt32(reader["CategoryId"]);

                        product.Category = new Category();
                        product.Category.CategoryId = product.CategoryId;
                        product.Category.Name = reader["CategoryName"].ToString();

                        productList.Add(product);
                    }
                }

                _connection.Close();
            }

            return productList;
        }

        public List<Category> GetAllCategories()
        {
            List<Category> categoryList = new List<Category>();

            string query = "SELECT * FROM Categories";

            using (SqlCommand command = new SqlCommand(query, _connection))
            {
                _connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Category category = new Category();

                        category.CategoryId =
                            Convert.ToInt32(reader["CategoryId"]);

                        category.Name =
                            reader["Name"].ToString();

                        categoryList.Add(category);
                    }
                }

                _connection.Close();
            }

            return categoryList;
        }
    }
}

