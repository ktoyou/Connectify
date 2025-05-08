namespace GachiHubBackend.Services.Janus;

public class JanusResponse
{
    public string Janus { get; set; } = default!;
    public string Transaction { get; set; } = default!;
    public JanusData Data { get; set; } = default!;
}