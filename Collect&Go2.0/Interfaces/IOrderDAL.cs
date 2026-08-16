using Collect_Go2._0.Models;

namespace Collect_Go2._0.Interfaces
{
    public interface IOrderDAL
    {
        Task<Order?> GetByIdAsync(int orderId);

        Task<List<Order>> GetByClientAsync(int clientId);

        Task CreateAsync(Order order);

        Task UpdateStatusAsync(int orderId, OrderStatus status);

        // Commandes d'un magasin, dans un statut donné, pour une date de créneau donnée
        Task<List<Order>> GetByStoreAndStatusAsync(int storeId,OrderStatus status,DateTime date);

        // Préparateur : marque la commande prête, précise le nombre de caisses utilisées
        Task MarkPreparedAsync( int orderId, int numberOfBoxes);

        // Caissier : encode les caisses rendues, fige le montant total, honore la commande
        Task ApproveOrderAsync( int orderId, int returnedBoxes, double totalAmount);
    }
}