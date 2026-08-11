using System.ComponentModel.DataAnnotations;

namespace Collect_Go2._0.Models
{
    public abstract class User
    {
        private int userId;

        public int UserId
        {
            get => userId;
            init
            {
                if (value < 0)
                    throw new ArgumentException(
                        "L'identifiant d'un utilisateur ne peut pas être négatif.");

                userId = value;
            }
        }

        private string email = string.Empty;

        
        public string Email
        {
            get => email;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException(
                        "L'email de l'utilisateur ne peut pas être vide.");

                email = value;
            }
        }

        private string firstname = string.Empty;
 
        public string Firstname
        {
            get => firstname;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException(
                        "Vous devez remplir cette case");

                firstname = value;
            }
        }

        private string lastname = string.Empty;

        public string Lastname
        {
            get => lastname;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException(
                        "Vous devez remplir cette case");

                lastname = value;
            }
        }

        private string password = string.Empty;

        [DataType(DataType.Password)]
        [StringLength(100,
            MinimumLength = 8,
            ErrorMessage = "Le mot de passe doit faire  08 caractères minimum")]
        public string Password
        {
            get => password;
            set => password = value;
        }

        public string UserType { get; set; } = string.Empty;



        public User(int userId,string firstname, string lastname, string email, string password)
        {
            UserId = userId;
            Firstname = firstname;
            Lastname = lastname;
            Email = email;
            Password = password;
        }
    }
}

