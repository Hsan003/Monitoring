namespace Monitoring.Repositories;

public interface IRepository<T> where T : class
{
    Task AddAsync(T entity);
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    void Update(T entity);
    void Delete(T entity);
    Task SaveAsync();

    void Delete(int id);
}
