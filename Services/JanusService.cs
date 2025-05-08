using GachiHubBackend.Services.Interfaces;
using GachiHubBackend.Services.Janus;

namespace GachiHubBackend.Services;

public class JanusService : IJanusService
{
    private readonly HttpClient _httpClient;

    public JanusService(HttpClient httpClient, IConfiguration configuration)
    {
        var janusDomain = configuration["Janus:Domain"];
        ArgumentNullException.ThrowIfNull(janusDomain);
        
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(janusDomain);
    }

    public async Task<long> CreateSessionAsync()
    {
        var response = await _httpClient.PostAsJsonAsync("", new { janus = "create", transaction = Guid.NewGuid().ToString() });
        var content = await response.Content.ReadFromJsonAsync<JanusResponse>();
        return content!.Data.Id;
    }

    public async Task<long> AttachToPluginAsync(long sessionId, string pluginName)
    {
        var response = await _httpClient.PostAsJsonAsync($"{sessionId}", new { janus = "attach", plugin = pluginName, transaction = Guid.NewGuid().ToString() });
        var content = await response.Content.ReadFromJsonAsync<JanusResponse>();
        return content!.Data.Id;
    }

    public async Task CreateRoomAsync(long sessionId, long pluginHandleId, long roomId)
    {
        var resp = await _httpClient.PostAsJsonAsync($"{sessionId}/{pluginHandleId}", new
        {
            janus = "message",
            body = new
            {
                request = "create",
                room = roomId,
                audiocodec = "opus",
                videocodec = "none"
            },
            transaction = Guid.NewGuid().ToString()
        });
    }

    public async Task JoinRoomAsync(long sessionId, long pluginHandleId, long roomId, int userId)
    {
        await _httpClient.PostAsJsonAsync($"{sessionId}/{pluginHandleId}", new
        {
            janus = "message",
            body = new
            {
                request = "join",
                room = roomId,
                ptype = "publisher",
                id = userId
            },
            transaction = Guid.NewGuid().ToString()
        });
    }
}
