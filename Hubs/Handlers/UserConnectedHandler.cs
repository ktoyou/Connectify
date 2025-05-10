using Connectify.Db.Model;
using GachiHubBackend.Hubs.Interfaces;
using GachiHubBackend.Repositories.Interfaces;
using GachiHubBackend.Services.Interfaces;

namespace GachiHubBackend.Hubs.Handlers;

public class UserConnectedHandler : IRoomHubConnectedHandler
{
    private readonly IRoomHubContextService _roomHubContextService;
    
    private readonly IRepository<User> _userRepository;
    
    public UserConnectedHandler(IRoomHubContextService roomHubContextService, IRepository<User> userRepository)
    {
        _roomHubContextService = roomHubContextService;
        _userRepository = userRepository;
    }
    
    public async Task HandleAsync(IRoomHubCaller caller)
    {
        var currentUser = await _roomHubContextService.GetCurrentUserAsync(caller);
        if (currentUser == null)
        {
            throw new UnauthorizedAccessException();
        }
        
        currentUser.ConnectionId = caller.Context.ConnectionId;
        await _userRepository.UpdateAsync(currentUser);
    }
}