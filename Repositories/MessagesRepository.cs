using Connectify.Db;
using Connectify.Db.Model;
using Microsoft.EntityFrameworkCore;

namespace GachiHubBackend.Repositories;

public class MessagesRepository : ConnectifyRepository<Message>
{
    public MessagesRepository(DbConnectifyContext context) : base(context) { }

    public async Task<(int totalCount, IEnumerable<Message> items)> GetMessagesByRoomId(int roomId, int pageNumber = 1, int pageSize = 10)
    {
        var items = await Context.Messages
                .Where(m => m.Room.Id == roomId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        var totalCount = await Context.Messages.CountAsync();
        
        return (totalCount, items);
    }
}