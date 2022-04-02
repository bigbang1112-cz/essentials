using BigBang1112.Models.Db;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BigBang1112.Repos;

public class Repo<TEntity> : IRepo<TEntity> where TEntity : DbModel
{
    private readonly DbContext _context;

    public Repo(DbContext context)
    {
        _context = context;
    }

    public void Add(TEntity entity)
    {
        if (entity is null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        _context.Set<TEntity>().Add(entity);
    }

    public void Delete(TEntity entity)
    {
        if (entity is null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        _context.Set<TEntity>().Remove(entity);
    }

    public IEnumerable<TEntity> GetAll()
    {
        return _context.Set<TEntity>().ToList();
    }

    public TEntity? GetById(int id)
    {
        return _context.Set<TEntity>().Find(id);
    }

    public void Update(TEntity entity)
    {
        _context.Set<TEntity>().Update(entity);
    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        if (entity is null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        await _context.Set<TEntity>().AddAsync(entity, cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Set<TEntity>().ToListAsync(cancellationToken);
    }

    public async Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<TEntity>().FindAsync(new object?[] { id }, cancellationToken);
    }

    public TEntity GetOrAdd<T>(Expression<Func<TEntity, bool>> predicate, Func<TEntity> creator)
    {
        var entity = _context.Set<TEntity>().FirstOrDefault(predicate);

        if (entity is null)
        {
            entity = creator();
            Add(entity);
        }

        return entity;
    }

    public async Task<TEntity> GetOrAddAsync<T>(Expression<Func<TEntity, bool>> predicate, Func<TEntity> creator, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Set<TEntity>().FirstOrDefaultAsync(predicate, cancellationToken);
        
        if (entity is null)
        {
            entity = creator();
            await AddAsync(entity, cancellationToken);
        }

        return entity;
    }

    public async Task<TEntity> GetOrAddAsync<T>(Expression<Func<TEntity, bool>> predicate, Func<Task<TEntity>> creator, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Set<TEntity>().FirstOrDefaultAsync(predicate, cancellationToken);

        if (entity is null)
        {
            entity = await creator();
            await AddAsync(entity, cancellationToken);
        }

        return entity;
    }
}