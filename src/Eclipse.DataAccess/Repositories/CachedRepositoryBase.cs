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

    public Task<TEntity?> FindAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return CacheService.GetOrCreateAsync(
            $"{GetPrefix()}-{id}",
            () => Repository.FindAsync(id, cancellationToken),
            CacheConsts.FiveMinutes,
            cancellationToken
        );
    }

    public Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return CacheService.GetOrCreateAsync(
            $"{GetPrefix()}-all",
            () => Repository.GetAllAsync(cancellationToken),
            CacheConsts.ThreeDays,
            cancellationToken
        );
    }

    public Task<IReadOnlyList<TEntity>> GetByExpressionAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
    {
        return CacheService.GetOrCreateAsync(
            $"{GetPrefix()};{expression.Body}",
            () => Repository.GetByExpressionAsync(expression, cancellationToken),
            CacheConsts.FiveMinutes,
            cancellationToken
        );
    }

    public Task<IReadOnlyList<TEntity>> GetByExpressionAsync(Expression<Func<TEntity, bool>> expression, int skipCount, int takeCount, CancellationToken cancellationToken = default)
    {
        return CacheService.GetOrCreateAsync(
            $"{GetPrefix()};{expression.Body};skip={skipCount};take={takeCount}",
            () => Repository.GetByExpressionAsync(expression, skipCount, takeCount, cancellationToken),
            CacheConsts.FiveMinutes,
            cancellationToken
        );
    }

    public virtual async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        entity = await Repository.UpdateAsync(entity, cancellationToken);

        await CacheService.DeleteByPrefixAsync(GetPrefix(), cancellationToken);

        return entity;
    }

    public async Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await Repository.UpdateRangeAsync(entities, cancellationToken);
        await CacheService.DeleteByPrefixAsync(GetPrefix(), cancellationToken);
    }

    public async Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await Repository.DeleteRangeAsync(entities, cancellationToken);
        await CacheService.DeleteByPrefixAsync(GetPrefix(), cancellationToken);
    }

    public async Task CreateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await Repository.CreateRangeAsync(entities, cancellationToken);
        await CacheService.DeleteByPrefixAsync(GetPrefix(), cancellationToken);
    }

    protected static string GetPrefix() => typeof(TEntity).AssemblyQualifiedName ?? string.Empty;
}
