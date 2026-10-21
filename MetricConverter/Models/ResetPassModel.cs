using System.ComponentModel.DataAnnotations;

namespace MetricConverter.Models
{
    public class ResetPassModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Token { get; set; }

        [Required(ErrorMessage = "Моля, въведете нова парола.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Паролите НЕ съвпадат.")]
        public string ConfirmPassword { get; set; }
    }
}
