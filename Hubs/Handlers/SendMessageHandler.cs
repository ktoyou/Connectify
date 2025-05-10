using Connectify.Db.Model;
using GachiHubBackend.Hubs.Enums;
using GachiHubBackend.Hubs.Interfaces;
using GachiHubBackend.Repositories.Interfaces;
using GachiHubBackend.Services.Interfaces;

namespace GachiHubBackend.Hubs.Handlers;

public class SendMessageHandler : IRoomHubHandler
{
    private readonly IRepository<Message> _messageRepository;
    
    private readonly IRoomHubContextService _roomHubContextService;
    
    public SendMessageHandler(IRepository<Message> messageRepository, IRoomHubContextService roomHubContextService)
    {
        _messageRepository = messageRepository;
        _roomHubContextService = roomHubContextService;
    }
    
    public async Task HandleAsync(IRoomHubCaller caller, object payload)
    {
        var currentUser = await _roomHubContextService.GetCurrentUserAsync(caller);
        var connectionIds = await _roomHubContextService.GetOtherUsersConnectionIdsInRoomAsync(caller);

        var message = new Message()
        {
            Content = payload.ToString(),
            Room = currentUser!.Room!,
            From = currentUser,
            SendAt = DateTime.Now,
        };
        await _messageRepository.AddAsync(message);

        await _roomHubContextService.SendToClientsAsync(connectionIds!, RoomHubEvent.ReceiveMessage, caller, new
        {
            From = currentUser,
            message.Content,
            message.SendAt,
        });
    }
}