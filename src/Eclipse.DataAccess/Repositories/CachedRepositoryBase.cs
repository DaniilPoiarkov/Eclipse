using Eclipse.Common.Cache;
using Eclipse.Domain.Shared.Entities;
using Eclipse.Domain.Shared.Repositories;

using System.Linq.Expressions;

namespace Eclipse.DataAccess.Repositories;

internal class CachedRepositoryBase<T> : IRepository<T>
    where T : Entity
{
    private readonly IRepository<T> _repository;

    private readonly ICacheService _cacheService;

    public CachedRepositoryBase(IRepository<T> repository, ICacheService cacheService)
    {
        _repository = repository;
        _cacheService = cacheService;
    }

    public Task<int> CountAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default)
    {
        return _repository.CountAsync(expression, cancellationToken);
    }

    public Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default)
    {
        return _repository.CreateAsync(entity, cancellationToken);
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _repository.DeleteAsync(id, cancellationToken);
    }

    public async Task<T?> FindAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var key = new CacheKey($"{typeof(T).AssemblyQualifiedName}-{id}");
        var entity = await _cacheService.GetAsync<T>(key, cancellationToken);

        if (entity is not null)
        {
            return entity;
        }

        entity = await _repository.FindAsync(id, cancellationToken);

        await _cacheService.SetAsync(key, entity, TimeSpan.FromMinutes(5), cancellationToken);

        return entity;
    }

    public Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _repository.GetAllAsync(cancellationToken);
    }

    public Task<IReadOnlyList<T>> GetByExpressionAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default)
    {
        return _repository.GetByExpressionAsync(expression, cancellationToken);
    }

    public Task<IReadOnlyList<T>> GetByExpressionAsync(Expression<Func<T, bool>> expression, int skipCount, int takeCount, CancellationToken cancellationToken = default)
    {
        return _repository.GetByExpressionAsync(expression, skipCount, takeCount, cancellationToken);
    }

    public async Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        var key = new CacheKey($"{typeof(T).AssemblyQualifiedName}-{entity.Id}");

        await _cacheService.DeleteAsync(key, cancellationToken);

        entity = await _repository.UpdateAsync(entity, cancellationToken);

        await _cacheService.SetAsync(key, entity, TimeSpan.FromMinutes(5), cancellationToken);

        return entity;
    }
}
