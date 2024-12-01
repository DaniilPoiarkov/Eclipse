using Eclipse.Common.Caching;
using Eclipse.DataAccess.Repositories;
using Eclipse.Domain.Users;

namespace Eclipse.DataAccess.Users;

internal sealed class CachedUserRepository : CachedRepositoryBase<User, IUserRepository>, IUserRepository
{
    public CachedUserRepository(IUserRepository repository, ICacheService cacheService)
        : base(repository, cacheService) { }

    public async Task<User?> FindByChatIdAsync(long chatId, CancellationToken cancellationToken = default)
    {
        var key = $"{GetPrefix()}-chat-id-{chatId}";
        var user = await CacheService.GetAsync<User>(key, cancellationToken);

        if (user is not null)
        {
            return user;
        }

        user = await Repository.FindByChatIdAsync(chatId, cancellationToken);

        await CacheService.SetAsync(key, user, CacheConsts.OneDay, cancellationToken);

        return user;
    }

    public async Task<User?> FindByUserNameAsync(string userName, CancellationToken cancellationToken = default)
    {
        var key = $"{GetPrefix()}-user-name-{userName}";
        var user = await CacheService.GetAsync<User>(key, cancellationToken);

        if (user is not null)
        {
            return user;
        }

        user = await Repository.FindByUserNameAsync(userName, cancellationToken);

        await CacheService.SetAsync(key, user, CacheConsts.OneDay, cancellationToken);

        return user;
    }
}
