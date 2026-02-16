using Answer.Application.Interfaces;
using Answer.Domain.Common;
using System.Collections.Concurrent;

namespace Answer.Infrastructure.Repositories;

public class InMemoryRepository<T> : IRepository<T> where T : BaseEntity
{
    private readonly ConcurrentDictionary<Guid, T> _entities = new();

    public Task<T?> GetByIdAsync(Guid id)
    {
        _entities.TryGetValue(id, out var entity);
        return Task.FromResult(entity);
    }

    public Task<IEnumerable<T>> GetAllAsync()
    {
        return Task.FromResult(_entities.Values.AsEnumerable());
    }

    public Task<T> AddAsync(T entity)
    {
        _entities.TryAdd(entity.Id, entity);
        return Task.FromResult(entity);
    }

    public Task UpdateAsync(T entity)
    {
        _entities.AddOrUpdate(entity.Id, entity, (key, oldValue) => entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id)
    {
        _entities.TryRemove(id, out _);
        return Task.CompletedTask;
    }
}
