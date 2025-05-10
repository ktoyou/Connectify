namespace GachiHubBackend.Hubs.Interfaces;

public interface IRoomHubHandler
{
    Task HandleAsync(IRoomHubCaller context, object payload);
}