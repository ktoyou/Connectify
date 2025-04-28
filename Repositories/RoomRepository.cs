using Connectify.Db;
using Connectify.Db.Model;
using Microsoft.EntityFrameworkCore;

namespace GachiHubBackend.Repositories;

public class RoomRepository : ConnectifyRepository<Room>
{
    public RoomRepository(DbConnectifyContext context) : base(context) { }
    
    public async Task<Room?> GetRoomByTitleAsync(string title)
        => await Context.Rooms.FirstOrDefaultAsync(r => r.Title == title);
    
    public async Task<Room?> GetRoomByOwnerIdAsync(int ownerId)
        => await Context.Rooms.FirstOrDefaultAsync(r => r.Owner.Id == ownerId);

    public async Task AddUserToRoomAsync(Room room, User user)
    {
        user.Room = room;
        await Context.SaveChangesAsync();
    }

    public async Task RemoveUserFromRoom(Room room, User user)
    {
        user.Room = null;
        await Context.SaveChangesAsync();
    }
}