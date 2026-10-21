using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MetricConverter.Models
{
    public class RegisterModel : IdentityErrorDescriber
    {
        [Required(ErrorMessage = "Потребителското име е ЗАДЪЛЖИТЕЛНО.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Имейл адресът е ЗАДЪЛЖИТЕЛЕН.")]
        [EmailAddress(ErrorMessage = "Невалиден имейл адрес.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Паролата е ЗАДЪЛЖИТЕЛНА.")]
        [MinLength(6, ErrorMessage = "Паролата трябва да бъде ПОНЕ 6 символа.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Потвърдете паролата.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Паролите НЕ съвпадат.")]
        public string ConfirmPassword { get; set; }
    }
}

