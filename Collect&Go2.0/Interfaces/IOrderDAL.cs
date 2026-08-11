using Collect_Go2._0.Models;

namespace Collect_Go2._0.Interfaces
{
    public interface IOrderDAL
    {
        Task<Order?> GetByIdAsync(int orderId);

        Task<List<Order>> GetByClientAsync(int clientId);

        Task CreateAsync(Order order);

        Task UpdateStatusAsync(
            int orderId,
            OrderStatus status);
    }
}