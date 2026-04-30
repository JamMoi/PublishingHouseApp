using System.ComponentModel.DataAnnotations;

namespace PublishingHouseApp.Models
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Salasanat eivät täsmää.")]
        public string ConfirmPassword { get; set; }
    }
}