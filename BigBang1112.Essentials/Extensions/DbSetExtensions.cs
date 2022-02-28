using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BigBang1112.Extensions;

public static class DbSetExtensions
{
    public static TEntity FirstOrAdd<TEntity>(this DbSet<TEntity> source,
        Expression<Func<TEntity, bool>> predicate, Func<TEntity> newItem) where TEntity : class
    {
        var existing = source.FirstOrDefault(predicate);

        if (existing is null)
        {
            existing = newItem.Invoke();
            source.Add(existing);
        }

        return existing;
    }

    public static async Task<TEntity> FirstOrAddAsync<TEntity>(this DbSet<TEntity> source,
        Expression<Func<TEntity, bool>> predicate, Func<TEntity> newItem,
        CancellationToken cancellationToken = default) where TEntity : class
    {
        var existing = await source.FirstOrDefaultAsync(predicate, cancellationToken);

        if (existing is null)
        {
            existing = newItem.Invoke();
            await source.AddAsync(existing, cancellationToken);
        }

        return existing;
    }

    public static async Task<TEntity> FirstOrAddAsync<TEntity>(this DbSet<TEntity> source,
        Expression<Func<TEntity, bool>> predicate, Func<Task<TEntity>> newItem,
        CancellationToken cancellationToken = default) where TEntity : class
    {
        var existing = await source.FirstOrDefaultAsync(predicate, cancellationToken);

        if (existing is null)
        {
            existing = await newItem.Invoke();
            await source.AddAsync(existing, cancellationToken);
        }

        return existing;
    }
}
