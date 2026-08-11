using Collect_Go2._0.Models;

namespace Collect_Go2._0.Interfaces
{
    public interface IProductDAL
    {
        Task<List<Product>> GetAllAsync();

        Task<List<Product>> GetByCategoryAsync(int categoryId);

        Task<Product?> GetByIdAsync(int productId);
    }
}