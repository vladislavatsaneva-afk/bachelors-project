using System.ComponentModel.DataAnnotations;

namespace MetricConverter.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Потребителското име е ЗАДЪЛЖИТЕЛНО.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Паролата е ЗАДЪЛЖИТЕЛНА.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public string NotiMessage { get; set; }
    }
}
