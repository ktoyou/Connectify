namespace GachiHubBackend.Services.Interfaces;

public interface IJanusService
{
    Task<long> CreateSessionAsync();
    Task<long> AttachToPluginAsync(long sessionId, string pluginName);
    Task CreateRoomAsync(long sessionId, long pluginHandleId, long roomId);
    Task JoinRoomAsync(long sessionId, long pluginHandleId, long roomId, int userId);
}
