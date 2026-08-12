using System.ComponentModel.DataAnnotations;

namespace Collect_Go2._0.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Le prénom est obligatoire.")]
        public string Firstname { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le nom est obligatoire.")]
        public string Lastname { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'email est obligatoire.")]
        [EmailAddress(ErrorMessage = "Adresse email invalide.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le mot de passe est obligatoire.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Le mot de passe doit faire 8 caractères minimum.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le téléphone est obligatoire.")]
        [Phone(ErrorMessage = "Numéro de téléphone invalide.")]
        public string Phone { get; set; } = string.Empty;
    }
}
