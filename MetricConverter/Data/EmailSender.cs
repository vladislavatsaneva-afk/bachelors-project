using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace MetricConverter.Data
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;

        public EmailSender(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
                var smtpHost = _config["Smtp:Host"];
                var smtpPort = int.Parse(_config["Smtp:Port"]);
                var enableSsl = bool.Parse(_config["Smtp:EnableSsl"] ?? "false");
                var username = _config["Smtp:UserName"];
                var password = _config["Smtp:Password"];

                var client = new SmtpClient(smtpHost, smtpPort)
                {
                    EnableSsl = enableSsl,
                    Credentials = CredentialCache.DefaultNetworkCredentials
                };

                var fromEmail = _config["Smtp:From"] ?? "reset@Converter.local";
                var mail = new MailMessage(fromEmail, toEmail, subject, body)
                {
                    IsBodyHtml = true
                };

                await client.SendMailAsync(mail);
        }
    }
}
