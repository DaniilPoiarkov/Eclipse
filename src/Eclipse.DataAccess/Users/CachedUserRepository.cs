using Eclipse.Common.Caching;
using Eclipse.DataAccess.Repositories;
using Eclipse.Domain.Users;

namespace Eclipse.DataAccess.Users;

internal sealed class CachedUserRepository : CachedRepositoryBase<User, IUserRepository>, IUserRepository
{
    public CachedUserRepository(IUserRepository repository, ICacheService cacheService)
        : base(repository, cacheService) { }

    public Task<User?> FindByChatIdAsync(long chatId, CancellationToken cancellationToken = default)
    {
        var options = new CacheOptions
        {
            Expiration = CacheConsts.OneDay,
            Tags = [GetPrefix()]
        };

        return CacheService.GetOrCreateAsync(
            $"{GetPrefix()}-chat-id-{chatId}",
            () => Repository.FindByChatIdAsync(chatId, cancellationToken),
            options,
            cancellationToken
        );
    }

    public Task<User?> FindByUserNameAsync(string userName, CancellationToken cancellationToken = default)
    {
        var options = new CacheOptions
        {
            Expiration = CacheConsts.OneDay,
            Tags = [GetPrefix()]
        };

        return CacheService.GetOrCreateAsync(
            $"{GetPrefix()}-user-name-{userName}",
            () => Repository.FindByUserNameAsync(userName, cancellationToken),
            options,
            cancellationToken
        );
    }
}
