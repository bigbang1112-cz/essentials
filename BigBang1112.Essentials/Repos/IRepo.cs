using BigBang1112.Models.Db;
using System.Linq.Expressions;

namespace BigBang1112.Repos;

public interface IRepo<TEntity> where TEntity : DbModel
{
    void Add(TEntity entity);
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    void AddRange(IEnumerable<TEntity> entities);
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    void Delete(TEntity entity);
    IEnumerable<TEntity> GetAll();
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    TEntity? GetById(int id);
    Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    TEntity GetOrAdd<T>(Expression<Func<TEntity, bool>> predicate, Func<TEntity> creator);
    Task<TEntity> GetOrAddAsync<T>(Expression<Func<TEntity, bool>> predicate, Func<TEntity> creator, CancellationToken cancellationToken = default);
    Task<TEntity> GetOrAddAsync<T>(Expression<Func<TEntity, bool>> predicate, Func<Task<TEntity>> creator, CancellationToken cancellationToken = default);
    void Update(TEntity entity);
}
