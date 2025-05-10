using Connectify.Db.Model;
using GachiHubBackend.Hubs.Enums;
using GachiHubBackend.Hubs.Interfaces;
using GachiHubBackend.Repositories.Interfaces;
using GachiHubBackend.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace GachiHubBackend.Hubs.Handlers;

public class UserDisconnectedHandler : IRoomHubDisconnectedHandler
{
    private readonly IRoomHubContextService _roomHubContextService;
    
    private readonly IRepository<User> _userRepository;
    
    public UserDisconnectedHandler(IRoomHubContextService roomHubContextService, IRepository<User> userRepository)
    {
        _roomHubContextService = roomHubContextService;
        _userRepository = userRepository;
    }
    
    public async Task HandleAsync(IRoomHubCaller caller, Exception? exception)
    {
        var currentUser = await _roomHubContextService.GetCurrentUserAsync(caller);
        if (currentUser!.Room != null)
        {
            var connectionIds = await _roomHubContextService.GetOtherUsersConnectionIdsInRoomAsync(caller);
            await caller.Clients.Clients(connectionIds!).SendAsync(RoomHubEvent.LeavedRoom.ToString(), currentUser, currentUser!.Room);
        
            currentUser!.ConnectionId = null;
            currentUser.RoomId = null;
            currentUser.Room = null;
        
            await _userRepository.UpdateAsync(currentUser);
        }
    }
}