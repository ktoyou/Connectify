namespace GachiHubBackend.Repositories.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    
    Task AddAsync(T entity);
    
    Task UpdateAsync(T entity);
    
    Task DeleteAsync(T entity);
    
    Task<(int totalCount, IEnumerable<T> items)> GetPageAsync(int pageNumber, int pageSize);
    
    Task<IEnumerable<T>> GetAllAsync();
}