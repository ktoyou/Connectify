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

    public async Task SendOffer(object offer)
    {
        var currentUser = await _roomHubContextService.GetCurrentUserAsync();
        var connectionIds = await _roomHubContextService.GetOtherUsersConnectionIdsInRoomAsync();
        await Clients.Clients(connectionIds!).SendAsync(RoomHubEvent.ReceiveOffer.ToString(), new
        {
            offer,
            fromUserId = currentUser!.Id,
        });
    }

    public async Task SendAnswer(object answer, int targetUserId)
    {
        var currentUser = await _roomHubContextService.GetCurrentUserAsync();
        var room = await _roomRepository.GetByIdAsync(currentUser!.Room!.Id);
        var targetUser = room!.Users.FirstOrDefault(u => u.Id == targetUserId);

        if (targetUser!.ConnectionId != null)
        {
            await Clients.Clients(targetUser.ConnectionId)
                .SendAsync(RoomHubEvent.ReceiveAnswer.ToString(), new
                {
                    answer,
                    fromUserId = currentUser!.Id,
                });    
        }
    }

    public async Task SendCandidate(object candidate)
    {
        var currentUser = await _roomHubContextService.GetCurrentUserAsync();
        var connectionIds = await _roomHubContextService.GetOtherUsersConnectionIdsInRoomAsync();
        await Clients.Clients(connectionIds!)
            .SendAsync(RoomHubEvent.ReceiveCandidate.ToString(), new
            {
                candidate,
                fromUserId = currentUser!.Id,
            });
    }
    
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
        await Clients.Clients(connectionIds!).SendAsync(RoomHubEvent.ReceiveMessage.ToString(), new
        {
            From = currentUser.Login,
            message.Content,
            message.SendAt,
        });
    }
    
    public async Task JoinRoom(int id)
    {
        var currentUser = await _roomHubContextService.GetCurrentUserAsync();
        var room = await _roomRepository.GetByIdAsync(id);
        if (room == null)
        {
            throw new HubException("Room not found");
        }
        
        await _roomRepository.AddUserToRoomAsync(room, currentUser!);
        
        var connectionIds = room.Users.Select(u => u.ConnectionId).ToList();

        await Clients.Clients(currentUser.ConnectionId).SendAsync(RoomHubEvent.InitiateOffer.ToString(), currentUser);
        await Clients.Clients(connectionIds!).SendAsync(RoomHubEvent.JoinedRoom.ToString(), currentUser, room);
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
        
        currentUser!.ConnectionId = null;
        currentUser.RoomId = null;
        currentUser.Room = null;
        
        var room = await _roomRepository.GetRoomByOwnerIdAsync(currentUser!.Id);
        if (room != null)
        {
            var connectionIds = room.Users.Select(u => u.ConnectionId);
            await Clients.Clients(connectionIds!).SendAsync(RoomHubEvent.LeftedRoom.ToString(), currentUser!.Login);
        }
        
        await _userRepository.UpdateAsync(currentUser);
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
}