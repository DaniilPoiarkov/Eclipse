using Eclipse.Application.Contracts.Users;
using Eclipse.Common.Cache;

namespace Eclipse.Application.Users;

internal sealed class UserCache : IUserCache
{
    private readonly ICacheService _cacheService;

    private static readonly CacheKey Key = new("Users");

    public UserCache(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task AddOrUpdateAsync(UserDto user, CancellationToken cancellationToken = default)
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

    public async Task<UserDto?> GetByChatIdAsync(long chatId, CancellationToken cancellationToken = default)
    {
        return (await GetListAsync(cancellationToken)).FirstOrDefault(u => u.ChatId == chatId);
    }

    public async Task<UserDto?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return (await GetListAsync(cancellationToken)).FirstOrDefault(u => u.Id == userId);
    }

    public async Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken cancellationToken = default) => (await GetListAsync(cancellationToken)).AsReadOnly();

    private async Task<List<UserDto>> GetListAsync(CancellationToken cancellationToken = default)
    {
        return (await _cacheService.GetAsync<List<UserDto>>(Key, cancellationToken)) ?? [];
    }
}
