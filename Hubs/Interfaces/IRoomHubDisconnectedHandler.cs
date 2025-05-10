namespace GachiHubBackend.Hubs.Interfaces;

public interface IRoomHubDisconnectedHandler
{
    Task HandleAsync(IRoomHubCaller caller, Exception? exception);
}