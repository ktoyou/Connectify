using GachiHubBackend.Hubs.Interfaces;

namespace GachiHubBackend.Services.Interfaces;

public interface IRoomHubHandlerService
{
    IRoomHubHandler? GetHandler(string action);
}