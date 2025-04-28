using Connectify.Db.Model;

namespace GachiHubBackend.Services.Interfaces;

public interface IRoomHubContextService
{
    Task<User?> GetCurrentUserAsync();

    Task<IEnumerable<string?>> GetOtherUsersConnectionIdsInRoomAsync();
}