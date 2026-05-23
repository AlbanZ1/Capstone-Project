using System.Net;
using System.Net.Mail;

namespace Auctions.Data.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SmtpEmailService> _logger;

        public SmtpEmailService(IConfiguration configuration, ILogger<SmtpEmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            if (!IsValidEmail(toEmail))
            {
                _logger.LogWarning("Skipped email with invalid recipient address: {Email}", toEmail);
                return;
            }

            var smtpSection = _configuration.GetSection("Email:Smtp");
            var host = smtpSection["Host"];
            var username = smtpSection["Username"];
            var password = smtpSection["Password"];
            var fromEmail = smtpSection["FromEmail"];
            var fromName = smtpSection["FromName"];

            if (string.IsNullOrWhiteSpace(host)
                || string.IsNullOrWhiteSpace(fromEmail)
                || string.IsNullOrWhiteSpace(username)
                || string.IsNullOrWhiteSpace(password))
            {
                _logger.LogWarning("SMTP email is not configured. Email to {Email} was not sent.", toEmail);
                return;
            }

            if (!IsValidEmail(fromEmail))
            {
                _logger.LogWarning("SMTP from address is invalid. Email to {Email} was not sent.", toEmail);
                return;
            }

            try
            {
                using var message = new MailMessage
                {
                    From = new MailAddress(fromEmail, string.IsNullOrWhiteSpace(fromName) ? fromEmail : fromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = false
                };
                message.To.Add(new MailAddress(toEmail));

                using var client = new SmtpClient(host)
                {
                    Port = smtpSection.GetValue("Port", 587),
                    EnableSsl = smtpSection.GetValue("EnableSsl", true),
                    Credentials = new NetworkCredential(username, password)
                };

                await client.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}.", toEmail);
            }
        }

        private static bool IsValidEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            try
            {
                var address = new MailAddress(email);
                return string.Equals(address.Address, email, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }
    }
}
