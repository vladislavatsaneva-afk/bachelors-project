using System.ComponentModel.DataAnnotations;

namespace MetricConverter.Models
{
    public class ForgotPassModel
    {
        [Required(ErrorMessage = "Имейлът е ЗАДЪЛЖИТЕЛЕН.")]
        [EmailAddress(ErrorMessage = "Невалиден имейл адрес.")]
        public string Email { get; set; }
    }
}
