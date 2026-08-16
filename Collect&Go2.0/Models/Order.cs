using Collect_Go2._0.Interfaces;

namespace Collect_Go2._0.Models
{
    public class Order
    {
        private int orderId;
        private DateTime orderDate;
        private double totalAmount;
        private int numberOfBoxes;

        public int OrderId
        {
            get => orderId;
            init
            {
                if (value < 0)
                    throw new ArgumentException(
                        "L'identifiant de la commande ne peut pas être négatif.");

                orderId = value;
            }
        }

        public DateTime OrderDate
        {
            get => orderDate;
            set => orderDate = value;
        }

        public double TotalAmount
        {
            get => totalAmount;
            private set => totalAmount = value;
        }

        public int NumberOfBoxes
        {
            get => numberOfBoxes;
            set
            {
                if (value < 0)
                    throw new ArgumentException(
                        "Le nombre de caisses ne peut pas être négatif.");

                numberOfBoxes = value;
            }
        }

        public OrderStatus Status { get; set; }

        public int? ReturnedBoxes { get; set; }

        public Client? Client { get; set; }

        public Store? Store { get; set; }

        public TimeSlot? TimeSlot { get; set; }

        public List<OrderItem> OrderItems { get; set; } = new();

        public Order()
        {
            Status = OrderStatus.Placed;
        }

        public Order(
            int orderId,
            DateTime orderDate,
            Client client,
            Store store,
            TimeSlot timeSlot)
        {
            OrderId = orderId;
            OrderDate = orderDate;
            Client = client;
            Store = store;
            TimeSlot = timeSlot;
            Status = OrderStatus.Placed;
        }

        public void AddItem(Product product, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException(
                    "La quantité doit être supérieure à zéro.");

            OrderItem? existingItem = OrderItems.FirstOrDefault(
                item => item.Product?.ProductId == product.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                OrderItems.Add(
                    new OrderItem(0, product, quantity));
            }

            TotalAmount = GetTotal();
        }

        public void RemoveItem(int productId)
        {
            OrderItems.RemoveAll(
                item => item.Product?.ProductId == productId);

            TotalAmount = GetTotal();
        }

        public double GetTotal()
        {
            return OrderItems.Sum(
                item => item.GetOrderItemTotal());
        }

        public string GetStatusLabel()
        {
            return Status switch
            {
                OrderStatus.Placed => "En attente de préparation",
                OrderStatus.Prepared => "Prête, à retirer",
                OrderStatus.Honored => "Retirée",
                _ => Status.ToString()
            };
        }

        // Réhydrate le montant final (frais inclus) tel que persisté en BDD, une fois la commande honorée
        public void SetPersistedTotal(double totalAmount)
        {
            TotalAmount = totalAmount;
        }

        public const double ServiceFee = 5.95;

        // Produits + frais de service + caution des caisses fournies - caisses rendues
        public double GetFinalTotal(int returnedBoxes)
        {
            return GetTotal() + ServiceFee + (ServiceFee * NumberOfBoxes) - (ServiceFee * returnedBoxes);
        }

        public Task PlaceOrderAsync(IOrderDAL orderDal)
        {
            return orderDal.CreateAsync(this);
        }

        public Task MarkPreparedAsync(IOrderDAL orderDal, int numberOfBoxes)
        {
            return orderDal.MarkPreparedAsync(OrderId, numberOfBoxes);
        }

        public Task ApproveOrderAsync(IOrderDAL orderDal, int returnedBoxes)
        {
            return orderDal.ApproveOrderAsync(OrderId, returnedBoxes, GetFinalTotal(returnedBoxes));
        }

        public static Task<Order?> GetByIdAsync(int orderId, IOrderDAL orderDal)
        {
            return orderDal.GetByIdAsync(orderId);
        }

        public static Task<List<Order>> GetByClientAsync(int clientId, IOrderDAL orderDal)
        {
            return orderDal.GetByClientAsync(clientId);
        }

        public static Task<List<Order>> GetByStoreAndStatusAsync(int storeId, OrderStatus status, DateTime date, IOrderDAL orderDal)
        {
            return orderDal.GetByStoreAndStatusAsync(storeId, status, date);
        }
    }
}