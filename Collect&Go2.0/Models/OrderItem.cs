namespace Collect_Go2._0.Models
{
    public class OrderItem
    {
        private int orderItemId;
        private int quantity;

        public int OrderItemId
        {
            get => orderItemId;
            init
            {
                if (value < 0)
                    throw new ArgumentException(
                        "L'identifiant de la ligne de commande ne peut pas être négatif.");

                orderItemId = value;
            }
        }

        public int Quantity
        {
            get => quantity;
            set
            {
                if (value <= 0)
                    throw new ArgumentException(
                        "La quantité doit être supérieure à zéro.");

                quantity = value;
            }
        }

        public Product? Product { get; set; }

        public Order? Order { get; set; }

        public OrderItem()
        {
        }

        public OrderItem(
            int orderItemId,
            Product product,
            int quantity)
        {
            OrderItemId = orderItemId;
            Product = product;
            Quantity = quantity;
        }

        public double GetOrderItemTotal()
        {
            if (Product == null)
                return 0;

            return Product.Price * Quantity;
        }
    }
}