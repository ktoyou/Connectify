using System.Text.Json.Serialization;

namespace Connectify.Db.Model;

public class User : BaseModel
{
    public int Id { get; set; }
    
    public string Login { get; set; } = string.Empty;

    [JsonIgnore]
    public string Password { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    [JsonIgnore]
    public virtual Room? Room { get; set; }

    public int? RoomId { get; set; }

    [JsonIgnore]
    public string? ConnectionId { get; set; } = string.Empty;
}