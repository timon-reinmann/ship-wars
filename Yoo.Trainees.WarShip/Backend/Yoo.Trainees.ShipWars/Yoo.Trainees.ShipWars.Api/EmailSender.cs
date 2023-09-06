using System.Net;
using System.Net.Mail;

namespace Yoo.Trainees.ShipWars.Api
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("smtp-mail.outlook.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("ShipWars@outlook.com", "Sh!pWars")
            };

            return client.SendMailAsync(
                new MailMessage(
                    from: "ShipWars@outlook.com",
                    to: email,
                    subject,
                    message
                )
            );
        }
    }
}
