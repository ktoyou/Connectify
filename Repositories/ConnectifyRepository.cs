using Connectify.Db;
using Connectify.Db.Model;
using GachiHubBackend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GachiHubBackend.Repositories;

public class ConnectifyRepository<T> : IRepository<T> where T : BaseModel
{
    protected readonly DbConnectifyContext Context;
    
    public ConnectifyRepository(DbConnectifyContext context)
    {
        Context = context;
    }

    public virtual async Task<T?> GetByIdAsync(int id)
        => await Context.Set<T>().FindAsync(id);

    public virtual async Task AddAsync(T entity)
    {
        await Context.Set<T>().AddAsync(entity);
        await Context.SaveChangesAsync();
    }

    public virtual async Task UpdateAsync(T entity)
    {
        Context.Set<T>().Update(entity);
        await Context.SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(T entity)
    {
        Context.Set<T>().Remove(entity);
        await Context.SaveChangesAsync();
    }

    public virtual async Task<(int totalCount, IEnumerable<T> items)> GetPageAsync(int pageNumber, int pageSize)
    {
        var items = await Context.Set<T>()
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
        var totalCount = await Context.Set<T>().CountAsync();
        
        return (totalCount, items);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
        => await Context.Set<T>().ToListAsync();
}