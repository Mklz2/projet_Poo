namespace Collect_Go2._0.Models
{
    public class Store
    {
        private int storeId;
        private string name = string.Empty;
        private string address = string.Empty;
        private string city = string.Empty;

        public int StoreId
        {
            get => storeId;
            init
            {
                if (value < 0)
                    throw new ArgumentException(
                        "L'identifiant du magasin ne peut pas être négatif.");

                storeId = value;
            }
        }

        public string Name
        {
            get => name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException(
                        "Le nom du magasin ne peut pas être vide.");

                name = value;
            }
        }

        public string Address
        {
            get => address;
            set => address = value;
        }

        public string City
        {
            get => city;
            set => city = value;
        }

        public List<Employee> Employees { get; set; } = new();

        public List<TimeSlot> TimeSlots { get; set; } = new();

        public List<Order> Orders { get; set; } = new();

        public Store()
        {
        }

        public Store(
            int storeId,
            string name,
            string address,
            string city)
        {
            StoreId = storeId;
            Name = name;
            Address = address;
            City = city;
        }
    }
}