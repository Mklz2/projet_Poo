namespace Collect_Go2._0.Models
{
    public class Product
    {
        private int productId;
        private string name = string.Empty;
        private double price;
        private string description = string.Empty;
        private string imageUrl = string.Empty;

        public int ProductId
        {
            get => productId;
            init
            {
                if (value < 0)
                    throw new ArgumentException(
                        "L'identifiant du produit ne peut pas être négatif.");

                productId = value;
            }
        }

        public string Name
        {
            get => name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException(
                        "Le nom du produit ne peut pas être vide.");

                name = value;
            }
        }

        public double Price
        {
            get => price;
            set
            {
                if (value < 0)
                    throw new ArgumentException(
                        "Le prix ne peut pas être négatif.");

                price = value;
            }
        }

        public string Description
        {
            get => description;
            set => description = value;
        }

        public string ImageUrl
        {
            get => imageUrl;
            set => imageUrl = value;
        }

        public Category? Category { get; set; }

        public List<OrderItem> OrderItems { get; set; } = new();

        public Product()
        {
        }

        public Product(
            int productId,
            string name,
            double price,
            string description,
            string imageUrl,
            Category category)
        {
            ProductId = productId;
            Name = name;
            Price = price;
            Description = description;
            ImageUrl = imageUrl;
            Category = category;
        }
    }
}