namespace Yoo.Trainees.ShipWars.Api
{
    public interface IChatClient
    {
        Task ReceiveMessage(string message);
    }
}
