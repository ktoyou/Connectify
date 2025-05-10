using GachiHubBackend.Hubs.Enums;
using GachiHubBackend.Hubs.Interfaces;
using GachiHubBackend.Repositories;
using GachiHubBackend.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace GachiHubBackend.Hubs.Handlers;

public class LeaveRoomHandler : IRoomHubHandler
{
    private readonly IRoomHubContextService _roomHubContextService;
    
    private readonly RoomRepository _roomRepository;
    
    private readonly UserRepository _userRepository;
    
    public LeaveRoomHandler(IRoomHubContextService roomHubContextService, RoomRepository roomRepository, UserRepository userRepository)
    {
        _roomHubContextService = roomHubContextService;
        _roomRepository = roomRepository;
        _userRepository = userRepository;
    }
    
    public async Task HandleAsync(IRoomHubCaller caller, object payload)
    {
        var currentUser = await _roomHubContextService.GetCurrentUserAsync(caller);
        var room = await _roomRepository.GetByIdAsync(currentUser!.Room!.Id);
        if (room == null)
        {
            throw new HubException("Room not found");
        }

        var user = room.Users.FirstOrDefault(u => u.ConnectionId == currentUser.ConnectionId);
        if (user == null)
        {
            throw new HubException("User not found");
        }
        
        var connectionIds = (await _roomHubContextService.GetOtherUsersConnectionIdsInRoomAsync(caller)).ToList();

        user.Room = null;
        user.RoomId = null;
        await _userRepository.UpdateAsync(user);
        
        await caller.Clients.Clients(connectionIds!).SendAsync(RoomHubEvent.LeavedRoom.ToString(), currentUser, room);
        await caller.Clients.AllExcept(connectionIds!).SendAsync(RoomHubEvent.LeavedFromOtherRoom.ToString(), room);
    }
}