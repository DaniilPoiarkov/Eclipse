using Eclipse.Common.Caching;
using Eclipse.DataAccess.Repositories;
using Eclipse.DataAccess.Repositories.Caching;
using Eclipse.Domain.ApiTokens;

namespace Eclipse.DataAccess.ApiTokens;

internal sealed class CachedApiTokenRepository : CachedRepositoryBase<ApiToken, IApiTokenRepository>, IApiTokenRepository
{
    public CachedApiTokenRepository(
        IApiTokenRepository repository,
        ICacheService cacheService,
        IExpressionHashStore expressionHashStore)
        : base(repository, cacheService, expressionHashStore) { }

    public Task<ApiToken?> FindByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default)
    {
        var options = new CacheOptions
        {
            Expiration = CacheConsts.FiveMinutes,
            Tags = [GetPrefix()]
        };

        return CacheService.GetOrCreateAsync(
            $"{GetPrefix()}-token-hash-{tokenHash}",
            () => Repository.FindByTokenHashAsync(tokenHash, cancellationToken),
            options,
            cancellationToken
        );
    }

    public Task<IReadOnlyList<ApiToken>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var options = new CacheOptions
        {
            Expiration = CacheConsts.FiveMinutes,
            Tags = [GetPrefix()]
        };

        return CacheService.GetOrCreateAsync(
            $"{GetPrefix()}-user-id-{userId}",
            () => Repository.GetByUserIdAsync(userId, cancellationToken),
            options,
            cancellationToken
        );
    }
}
