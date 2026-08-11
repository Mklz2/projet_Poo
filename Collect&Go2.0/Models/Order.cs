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

        public double GetTotal()
        {
            return OrderItems.Sum(
                item => item.GetOrderItemTotal());
        }
    }
}