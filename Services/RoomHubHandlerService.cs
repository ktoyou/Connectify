using GachiHubBackend.Hubs.Interfaces;
using GachiHubBackend.Services.Interfaces;

namespace GachiHubBackend.Services;

public class RoomHubHandlerService : IRoomHubHandlerService
{
    private readonly IDictionary<string, IRoomHubHandler> _handlers;

    public RoomHubHandlerService(IEnumerable<IRoomHubHandler> handlers)
    {
        _handlers = handlers.ToDictionary(h => h.GetType().Name.Replace("Handler", ""), h => h);
    }
    
    public IRoomHubHandler? GetHandler(string action)
    {
        _handlers.TryGetValue(action, out var handler);
        return handler;
    }
}