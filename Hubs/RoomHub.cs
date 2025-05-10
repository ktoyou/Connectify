using GachiHubBackend.Hubs.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;

namespace GachiHubBackend.Hubs;

[Authorize]
[SignalRHub]
public class RoomHub : Hub
{
    private readonly IDictionary<string, IRoomHubHandler> _roomHubHandlers;
    
    private readonly IRoomHubConnectedHandler _roomHubConnectedHandler;
    
    private readonly IRoomHubDisconnectedHandler _roomHubDisconnectedHandler;
    
    public RoomHub(IEnumerable<IRoomHubHandler> handlers, IRoomHubConnectedHandler connectedHandler, IRoomHubDisconnectedHandler disconnectedHandler)
    {
        _roomHubConnectedHandler = connectedHandler;
        _roomHubDisconnectedHandler = disconnectedHandler;
        _roomHubHandlers = handlers.ToDictionary(h => h.GetType().Name.Replace("Handler", ""), h => h);
    }

    public async Task Handle(string action, object payload)
    {
        if (_roomHubHandlers.TryGetValue(action, out var handler))
        {
            var caller = new RoomHubCaller(Clients, Context, Groups);
            await handler.HandleAsync(caller, payload);
        }
        else
        {
            throw new HubException($"{action} not found");
        }
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var caller = new RoomHubCaller(Clients, Context, Groups);
        await _roomHubDisconnectedHandler.HandleAsync(caller, exception);
        
        await base.OnDisconnectedAsync(exception);
    }

    public override async Task OnConnectedAsync()
    {
        var caller = new RoomHubCaller(Clients, Context, Groups);
        await _roomHubConnectedHandler.HandleAsync(caller);
        
        await base.OnConnectedAsync();
    }
}