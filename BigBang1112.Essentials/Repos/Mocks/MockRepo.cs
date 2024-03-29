﻿using System.Linq.Expressions;
using BigBang1112.Models.Db;

namespace BigBang1112.Repos.Mocks;

public class MockRepo<TEntity> : IRepo<TEntity> where TEntity : DbModel
{
    public List<TEntity> Entities { get; init; }

    public MockRepo()
    {
        Entities = new List<TEntity>();
    }

    public void Add(TEntity entity)
    {
        Entities.Add(entity);
    }

    public Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        Add(entity);
        return Task.CompletedTask;
    }

    public void AddRange(IEnumerable<TEntity> entities)
    {
        Entities.AddRange(entities);
    }

    public Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        AddRange(entities);
        return Task.CompletedTask;
    }

    public void Delete(TEntity entity)
    {
        Entities.Remove(entity);
    }

    public IEnumerable<TEntity> GetAll()
    {
        return Entities;
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(GetAll());
    }

    public TEntity? GetById(int id)
    {
        return Entities.SingleOrDefault(x => x.Id == id);
    }

    public async Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(GetById(id));
    }

    public TEntity GetOrAdd(Expression<Func<TEntity, bool>> predicate, Func<TEntity> creator)
    {
        var entity = Entities.FirstOrDefault(predicate.Compile());

        if (entity is null)
        {
            entity = creator();
            Add(entity);
        }

        return entity;
    }

    public async Task<TEntity> GetOrAddAsync(Expression<Func<TEntity, bool>> predicate, Func<TEntity> creator, CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(GetOrAdd(predicate, creator));
    }

    public async Task<TEntity> GetOrAddAsync(Expression<Func<TEntity, bool>> predicate, Func<Task<TEntity>> creator, CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(GetOrAdd(predicate, () => creator().Result));
    }

    public void Update(TEntity entity)
    {
        throw new NotImplementedException();
    }
}
