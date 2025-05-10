namespace GachiHubBackend.Hubs.Interfaces;

public interface IRoomHubConnectedHandler
{
    Task HandleAsync(IRoomHubCaller caller);
}