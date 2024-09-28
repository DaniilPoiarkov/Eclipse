using Eclipse.Common.Caching;
using Eclipse.Domain.Shared.Entities;
using Eclipse.Domain.Shared.Repositories;

using System.Linq.Expressions;

namespace Eclipse.DataAccess.Repositories;

internal class CachedRepositoryBase<TEntity, TRepository> : IRepository<TEntity>
    where TEntity : Entity
    where TRepository : class, IRepository<TEntity>
{
    protected readonly TRepository Repository;

    protected readonly ICacheService CacheService;

    public CachedRepositoryBase(TRepository repository, ICacheService cacheService)
    {
        Repository = repository;
        CacheService = cacheService;
    }

    public Task<int> CountAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
    {
        return Repository.CountAsync(expression, cancellationToken);
    }

    public async Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await CacheService.DeleteByPrefixAsync(GetPrefix(), cancellationToken);
        return await Repository.CreateAsync(entity, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await Repository.DeleteAsync(id, cancellationToken);
        await CacheService.DeleteByPrefixAsync(GetPrefix(), cancellationToken);
    }

    public async Task<TEntity?> FindAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var key = new CacheKey($"{GetPrefix()}-{id}");
        var entity = await CacheService.GetAsync<TEntity>(key, cancellationToken);

        if (entity is not null)
        {
            return entity;
        }

        entity = await Repository.FindAsync(id, cancellationToken);

        await CacheService.SetAsync(key, entity, CacheConsts.FiveMinutes, cancellationToken);

        return entity;
    }

    public async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var key = new CacheKey($"{GetPrefix()}-all");

        var cached = await CacheService.GetAsync<List<TEntity>>(key, cancellationToken);

        if (cached is not null)
        {
            return cached;
        }

        var items = await Repository.GetAllAsync(cancellationToken);

        await CacheService.SetAsync(key, items, CacheConsts.ThreeDays, cancellationToken);

        return items;
    }

    public async Task<IReadOnlyList<TEntity>> GetByExpressionAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
    {
        var key = new CacheKey($"{GetPrefix()};{expression.Body}");

        var cached = await CacheService.GetAsync<List<TEntity>>(key, cancellationToken);

        if (cached is not null)
        {
            return cached;
        }

        var items = await Repository.GetByExpressionAsync(expression, cancellationToken);

        await CacheService.SetAsync(key, items, CacheConsts.FiveMinutes, cancellationToken);

        return items;
    }

    public async Task<IReadOnlyList<TEntity>> GetByExpressionAsync(Expression<Func<TEntity, bool>> expression, int skipCount, int takeCount, CancellationToken cancellationToken = default)
    {
        var key = new CacheKey($"{GetPrefix()};{expression.Body};skip={skipCount};take={takeCount}");

        var cached = await CacheService.GetAsync<List<TEntity>>(key, cancellationToken);

        if (cached is not null)
        {
            return cached;
        }

        var items = await Repository.GetByExpressionAsync(expression, skipCount, takeCount, cancellationToken);

        await CacheService.SetAsync(key, items, CacheConsts.FiveMinutes, cancellationToken);

        return items;
    }

    public virtual async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var key = new CacheKey($"{GetPrefix()}-{entity.Id}");

        await CacheService.DeleteAsync(key, cancellationToken);

        entity = await Repository.UpdateAsync(entity, cancellationToken);

        await CacheService.DeleteByPrefixAsync(GetPrefix(), cancellationToken);
        await CacheService.SetAsync(key, entity, CacheConsts.FiveMinutes, cancellationToken);

        return entity;
    }

    public async Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await CacheService.DeleteByPrefixAsync(GetPrefix(), cancellationToken);
        await Repository.UpdateRangeAsync(entities, cancellationToken);
    }

    public async Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await CacheService.DeleteByPrefixAsync(GetPrefix(), cancellationToken);
        await Repository.DeleteRangeAsync(entities, cancellationToken);
    }

    protected static string GetPrefix() => typeof(TEntity).AssemblyQualifiedName ?? string.Empty;
}
