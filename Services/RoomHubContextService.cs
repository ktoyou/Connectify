using System.Security.Claims;
using Connectify.Db.Model;
using GachiHubBackend.Hubs;
using GachiHubBackend.Repositories;
using GachiHubBackend.Services.Interfaces;

namespace GachiHubBackend.Services;

public class RoomHubContextService : IRoomHubContextService
{
    private readonly RoomHub _roomHub;
    
    private readonly UserRepository _userRepository;

    public RoomHubContextService(RoomHub roomHub, UserRepository userRepository)
    {
        _roomHub = roomHub;
        _userRepository = userRepository;
    }
    
    public async Task<User?> GetCurrentUserAsync()
    {
        var login = _roomHub.Context.User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        return await _userRepository.GetUserByLoginAsync(login!);
    }

    public async Task<IEnumerable<string?>> GetOtherUsersConnectionIdsInRoomAsync()
    {
        var currentUser = await GetCurrentUserAsync();
        var connectionIds = currentUser!.Room!.Users
            .Where(u => u.ConnectionId != currentUser.ConnectionId)
            .Select(u => u.ConnectionId);

        return connectionIds;
    }
}