namespace Connectify.Db.Model;

public class Room : BaseModel
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;
    
    public virtual User Owner { get; set; } = null!;
    
    public virtual ICollection<Message> Messages { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}