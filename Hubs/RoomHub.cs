using GachiHubBackend.Hubs.Enums;
using GachiHubBackend.Hubs.Interfaces;
using GachiHubBackend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;

namespace GachiHubBackend.Hubs;

[Authorize]
[SignalRHub]
public class RoomHub : Hub
{
    private readonly IRoomHubConnectedHandler _roomHubConnectedHandler;
    
    private readonly IRoomHubDisconnectedHandler _roomHubDisconnectedHandler;

    private readonly IRoomHubHandlerService _handlerService;
    
    public RoomHub(IRoomHubHandlerService handlerService, IRoomHubConnectedHandler connectedHandler, IRoomHubDisconnectedHandler disconnectedHandler)
    {
        _handlerService = handlerService;
        _roomHubConnectedHandler = connectedHandler;
        _roomHubDisconnectedHandler = disconnectedHandler;
    }

    public async Task Handle(string action, object payload)
    {
        var handler = _handlerService.GetHandler(action);
        if (handler != null)
        {
            await handler.HandleAsync(new RoomHubCaller(Clients, Context, Groups), payload);
            return;
        }
        
        await Clients.Caller.SendAsync(RoomHubEvent.MethodNotFound.ToString(), $"Method {action} not found");
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await _roomHubDisconnectedHandler.HandleAsync(new RoomHubCaller(Clients, Context, Groups), exception);
        await base.OnDisconnectedAsync(exception);
    }

    public override async Task OnConnectedAsync()
    {
        await _roomHubConnectedHandler.HandleAsync(new RoomHubCaller(Clients, Context, Groups));
        await base.OnConnectedAsync();
    }
}