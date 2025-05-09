using Connectify.Db.Model;
using GachiHubBackend.Hubs.Enums;
using GachiHubBackend.Repositories;
using GachiHubBackend.Repositories.Interfaces;
using GachiHubBackend.Services;
using GachiHubBackend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;

namespace GachiHubBackend.Hubs;

[Authorize]
[SignalRHub]
public class RoomHub : Hub
{
    private readonly UserRepository _userRepository;

    private readonly RoomRepository _roomRepository;

    private readonly IRepository<Message> _messageRepository;

    private readonly IRoomHubContextService _roomHubContextService;
    
    public RoomHub(UserRepository userRepository, RoomRepository roomRepository, IRepository<Message> messagesRepository)
    {
        _userRepository = userRepository;
        _roomRepository = roomRepository;
        _messageRepository = messagesRepository;
        _roomHubContextService = new RoomHubContextService(this, _userRepository);
    }

    public async Task StopScreenShare() => await NotifyRoomUsersAsync(RoomHubEvent.ScreenShareStopped);

    public async Task StopCameraShare() => await NotifyRoomUsersAsync(RoomHubEvent.CameraShareStopped);

    public async Task StartScreenShare() => await NotifyRoomUsersAsync(RoomHubEvent.ScreenShareStarted);

    public async Task StartCameraShare() => await NotifyRoomUsersAsync(RoomHubEvent.CameraShareStarted);
    
    public async Task SendMessage(string content)
    {
        var currentUser = await _roomHubContextService.GetCurrentUserAsync();
        var connectionIds = await _roomHubContextService.GetOtherUsersConnectionIdsInRoomAsync();

        var message = new Message()
        {
            Content = content,
            Room = currentUser!.Room!,
            From = currentUser,
            SendAt = DateTime.Now,
        };
        await _messageRepository.AddAsync(message);

        await SendToClientsAsync(connectionIds!, RoomHubEvent.ReceiveMessage, new
        {
            From = currentUser,
            message.Content,
            message.SendAt,
        });
    }
    
    public async Task JoinRoom(int id)
    {
        var currentUser = await _roomHubContextService.GetCurrentUserAsync();
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
        
        var connectionIds = (await _roomHubContextService.GetOtherUsersConnectionIdsInRoomAsync()).ToList();
        
        await Clients.Clients(connectionIds!).SendAsync(RoomHubEvent.JoinedRoom.ToString(), new
        {
            currentUser = currentUser,
            room = room,
            createOffer = false
        });
        
        await Clients.Caller.SendAsync(RoomHubEvent.JoinedRoom.ToString(), new
        {
            currentUser = currentUser,
            room = room,
            createOffer = true
        });
        
        await Clients.AllExcept(connectionIds!).SendAsync(RoomHubEvent.JoinedToOtherRoom.ToString(), room);
    }

    public async Task LeaveRoom()
    {
        var currentUser = await _roomHubContextService.GetCurrentUserAsync();
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
        
        var connectionIds = (await _roomHubContextService.GetOtherUsersConnectionIdsInRoomAsync()).ToList();

        user.Room = null;
        user.RoomId = null;
        await _userRepository.UpdateAsync(user);
        
        await Clients.Clients(connectionIds!).SendAsync(RoomHubEvent.LeavedRoom.ToString(), currentUser, room);
        await Clients.AllExcept(connectionIds!).SendAsync(RoomHubEvent.LeavedFromOtherRoom.ToString(), room);
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var currentUser = await _roomHubContextService.GetCurrentUserAsync();
        if (currentUser!.Room != null)
        {
            var connectionIds = await _roomHubContextService.GetOtherUsersConnectionIdsInRoomAsync();
            await Clients.Clients(connectionIds!).SendAsync(RoomHubEvent.LeavedRoom.ToString(), currentUser, currentUser!.Room);
        
            currentUser!.ConnectionId = null;
            currentUser.RoomId = null;
            currentUser.Room = null;
        
            await _userRepository.UpdateAsync(currentUser);
        }
        
        await base.OnDisconnectedAsync(exception);
    }

    public override async Task OnConnectedAsync()
    {
        var currentUser = await _roomHubContextService.GetCurrentUserAsync();
        if (currentUser == null)
        {
            throw new UnauthorizedAccessException();
        }
        
        currentUser.ConnectionId = Context.ConnectionId;
        await _userRepository.UpdateAsync(currentUser);
        
        await base.OnConnectedAsync();
    }

    private async Task NotifyRoomUsersAsync(RoomHubEvent hubEvent)
    {
        var user = await _roomHubContextService.GetCurrentUserAsync();
        var connectionIds = await _roomHubContextService.GetOtherUsersConnectionIdsInRoomAsync();
        if (user != null)
        {
            await Clients.Clients(connectionIds!).SendAsync(hubEvent.ToString(), user.Id);
        }
    }

    private async Task SendToClientsAsync(IEnumerable<string> connectionIds, RoomHubEvent hubEvent, object payload)
        => await Clients.Clients(connectionIds!).SendAsync(hubEvent.ToString(), payload);
    
    private async Task SendToClientAsync(string connectionId, RoomHubEvent hubEvent, object payload)
        => await Clients.Client(connectionId).SendAsync(hubEvent.ToString(), payload);
}