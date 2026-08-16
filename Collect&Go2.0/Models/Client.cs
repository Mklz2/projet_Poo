using Collect_Go2._0.Interfaces;

namespace Collect_Go2._0.Models
{
    public class Client : User
    {
        private string phone = string.Empty;

        public string Phone
        {
            get => phone;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException(
                        "Le numéro de téléphone ne peut pas être vide.");

                phone = value;
            }
        }

        public List<Order> Orders { get; set; } = new();

        public override string UserType => "client";

        public Client(
            int userId,
            string firstname,
            string lastname,
            string email,
            string password,
            string phone)
            : base(userId, firstname, lastname, email, password)
        {
            Phone = phone;
        }

        public static Task<bool> EmailExistsAsync(string email, IUserDAL userDal)
        {
            return userDal.EmailExistsAsync(email);
        }

        public Task CreateAccountAsync(IUserDAL userDal)
        {
            Password = BCrypt.Net.BCrypt.HashPassword(Password);
            return userDal.CreateAsync(this);
        }
    }
}