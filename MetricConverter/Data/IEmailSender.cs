using System.Threading.Tasks;

namespace MetricConverter.Data
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}
