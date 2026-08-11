using Collect_Go2._0.Models;

namespace Collect_Go2._0.Interfaces
{
    public interface ICategoryDAL
    {
        Task<List<Category>> GetAllAsync();

        Task<Category?> GetByIdAsync(int categoryId);
    }
}