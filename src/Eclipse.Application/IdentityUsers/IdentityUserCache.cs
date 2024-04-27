using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Common.Cache;

namespace Eclipse.Application.IdentityUsers;

internal sealed class IdentityUserCache : IIdentityUserCache
{
    private readonly ICacheService _cacheService;

    private static readonly CacheKey Key = new("Identity-Users");

    public IdentityUserCache(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task AddOrUpdateAsync(IdentityUserDto user, CancellationToken cancellationToken = default)
    {
        var users = await GetListAsync(cancellationToken);

        var existing = users.FirstOrDefault(u => u.Id == user.Id);

        if (existing is not null)
        {
            users.Remove(existing);
        }

        users.Add(user);

        await _cacheService.SetAsync(Key, users, cancellationToken);
    }

    public async Task<IdentityUserDto?> GetByChatIdAsync(long chatId, CancellationToken cancellationToken = default)
    {
        return (await GetListAsync(cancellationToken)).FirstOrDefault(u => u.ChatId == chatId);
    }

    public async Task<IdentityUserDto?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return (await GetListAsync(cancellationToken)).FirstOrDefault(u => u.Id == userId);
    }

    public async Task<IReadOnlyList<IdentityUserDto>> GetAllAsync(CancellationToken cancellationToken = default) => (await GetListAsync(cancellationToken)).AsReadOnly();

    private async Task<List<IdentityUserDto>> GetListAsync(CancellationToken cancellationToken = default)
    {
        return (await _cacheService.GetAsync<List<IdentityUserDto>>(Key, cancellationToken)) ?? [];
    }
}
