using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Yoo.Trainees.ShipWars.Api
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;

        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_emailSettings.Email, _emailSettings.Password)
            };

            return client.SendMailAsync(
                new MailMessage(
                    from: _emailSettings.Email,
                    to: email,
                    subject,
                    message
                )
            );
        }
    }
}
