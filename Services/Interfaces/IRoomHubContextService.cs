using Connectify.Db.Model;
using GachiHubBackend.Hubs.Enums;
using GachiHubBackend.Hubs.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace GachiHubBackend.Services.Interfaces;

public interface IRoomHubContextService
{
    Task<User?> GetCurrentUserAsync(IRoomHubCaller context);

    Task<IEnumerable<string?>> GetOtherUsersConnectionIdsInRoomAsync(IRoomHubCaller context);

    Task SendToClientsAsync(IEnumerable<string> connectionIds, RoomHubEvent hubEvent, IRoomHubCaller caller, object payload);

    Task SendToClientAsync(string connectionId, RoomHubEvent hubEvent, IRoomHubCaller caller, object payload);
}