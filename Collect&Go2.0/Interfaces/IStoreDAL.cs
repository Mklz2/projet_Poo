using Collect_Go2._0.Models;

namespace Collect_Go2._0.Interfaces
{
    public interface IStoreDAL
    {
        Task<List<Store>> GetAllAsync();

        Task<Store?> GetByIdAsync(int storeId);
    }
}