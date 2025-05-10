using System.Security.Claims;
using Connectify.Db.Model;
using GachiHubBackend.Hubs;
using GachiHubBackend.Hubs.Enums;
using GachiHubBackend.Hubs.Interfaces;
using GachiHubBackend.Repositories;
using GachiHubBackend.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace GachiHubBackend.Services;

public class RoomHubContextService : IRoomHubContextService
{
    private readonly UserRepository _userRepository;

    public RoomHubContextService(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<User?> GetCurrentUserAsync(IRoomHubCaller caller)
    {
        var login = caller.Context.User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        return await _userRepository.GetUserByLoginAsync(login!);
    }

    public async Task<IEnumerable<string?>> GetOtherUsersConnectionIdsInRoomAsync(IRoomHubCaller caller)
    {
        var currentUser = await GetCurrentUserAsync(caller);
        var connectionIds = currentUser!.Room!.Users
            .Where(u => u.ConnectionId != currentUser.ConnectionId)
            .Select(u => u.ConnectionId);

        return connectionIds;
    }

    public async Task SendToClientsAsync(IEnumerable<string> connectionIds, RoomHubEvent hubEvent, IRoomHubCaller caller, object payload)
        => await caller.Clients.Clients(connectionIds!).SendAsync(hubEvent.ToString(), payload);
    
    public async Task SendToClientAsync(string connectionId, RoomHubEvent hubEvent, IRoomHubCaller caller, object payload)
        => await caller.Clients.Client(connectionId).SendAsync(hubEvent.ToString(), payload);
}