using GachiHubBackend.Hubs.Enums;
using GachiHubBackend.Hubs.Interfaces;
using GachiHubBackend.Repositories;
using GachiHubBackend.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace GachiHubBackend.Hubs.Handlers;

public class JoinRoomHandler : IRoomHubHandler
{
    private IRoomHubContextService _roomHubContextService;
    
    private RoomRepository _roomRepository;
    
    public JoinRoomHandler(IRoomHubContextService roomHubContextService, RoomRepository roomRepository)
    {
        _roomHubContextService = roomHubContextService;
        _roomRepository = roomRepository;
    }
    
    public async Task HandleAsync(IRoomHubCaller caller, object payload)
    {
        var id = int.Parse(payload.ToString()!);
        
        var currentUser = await _roomHubContextService.GetCurrentUserAsync(caller);
        if (currentUser!.Room != null)
        {
            return;
        }
        
        var room = await _roomRepository.GetByIdAsync(id);
        if (room == null)
        {
            throw new HubException("Room not found");
        }
        
        await _roomRepository.AddUserToRoomAsync(room, currentUser!);
        
        var connectionIds = (await _roomHubContextService.GetOtherUsersConnectionIdsInRoomAsync(caller)).ToList();
        
        await caller.Clients.Clients(connectionIds!).SendAsync(RoomHubEvent.JoinedRoom.ToString(), new
        {
            currentUser = currentUser,
            room = room,
            createOffer = false
        });
        
        await caller.Clients.Caller.SendAsync(RoomHubEvent.JoinedRoom.ToString(), new
        {
            currentUser = currentUser,
            room = room,
            createOffer = true
        });
        
        await caller.Clients.AllExcept(connectionIds!).SendAsync(RoomHubEvent.JoinedToOtherRoom.ToString(), room);
    }
}