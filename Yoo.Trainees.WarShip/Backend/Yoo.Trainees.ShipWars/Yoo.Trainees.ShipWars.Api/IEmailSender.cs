namespace Yoo.Trainees.ShipWars.Api
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
