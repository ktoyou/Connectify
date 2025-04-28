using Connectify.Db;
using Connectify.Db.Model;
using Microsoft.EntityFrameworkCore;

namespace GachiHubBackend.Repositories;

public class UserRepository : ConnectifyRepository<User>
{
    public UserRepository(DbConnectifyContext context) : base(context) { }

    public async Task<User?> GetUserByLoginAndPasswordAsync(string username, string password)
    {
        var user = await Context.Users.FirstOrDefaultAsync(u => u.Login == username && u.Password == password);
        return user;
    }

    public async Task<User?> GetUserByLoginAsync(string login)
    {
        var user = await Context.Users.FirstOrDefaultAsync(u => u.Login == login);
        return user;
    }
}