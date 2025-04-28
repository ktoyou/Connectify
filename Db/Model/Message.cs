using System.Text.Json.Serialization;

namespace Connectify.Db.Model;

public class Message : BaseModel
{
    public int Id { get; set; }
    
    public virtual User From { get; set; } = new User();
    
    public DateTime SendAt { get; set; }

    [JsonIgnore]
    public virtual Room Room { get; set; } = new Room();

    public string Content { get; set; } = string.Empty;
}