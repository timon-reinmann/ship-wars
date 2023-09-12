using Microsoft.Extensions.Options;
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

            var client = new SmtpClient(configuration["SmtpClient:Host"], int.Parse(configuration["SmtpClient:Port"]))
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(configuration["NetworkCredential:UserName"], configuration["NetworkCredential:Password"])
            };

            await client.SendMailAsync(
                new MailMessage(
                    from: configuration["NetworkCredential:UserName"],
                    to: email,
                    subject,
                    message
                )
            );
        }
    }
}
