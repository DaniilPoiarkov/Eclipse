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
        return CacheService.GetOrCreateAsync(
            $"{GetPrefix()}-chat-id-{chatId}",
            () => Repository.FindByChatIdAsync(chatId, cancellationToken),
            CacheConsts.OneDay,
            cancellationToken
        );
    }

    public Task<User?> FindByUserNameAsync(string userName, CancellationToken cancellationToken = default)
    {
        return CacheService.GetOrCreateAsync(
            $"{GetPrefix()}-user-name-{userName}",
            () => Repository.FindByUserNameAsync(userName, cancellationToken),
            CacheConsts.OneDay,
            cancellationToken
        );
    }
}
