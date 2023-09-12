using System.Net;
using System.Net.Mail;

namespace Yoo.Trainees.ShipWars.Api
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration configuration;

        public EmailSender(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {

            var client = new SmtpClient(configuration["EmailSettings:SmtpClient:Host"], int.Parse(configuration["EmailSettings:SmtpClient:Port"]))
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(configuration["EmailSettings:NetworkCredential:serName"], configuration["EmailSettings:NetworkCredential:password"])
            };

            await client.SendMailAsync(
                new MailMessage(
                    from: configuration["EmailSettings:NetworkCredential:userName"],
                    to: email,
                    subject,
                    message
                )
            );
        }
    }
}
