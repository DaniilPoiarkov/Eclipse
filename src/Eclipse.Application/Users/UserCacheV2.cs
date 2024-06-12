using Eclipse.Application.Caching;
using Eclipse.Application.Contracts.Users;
using Eclipse.Common.Cache;

namespace Eclipse.Application.Users;

internal sealed class UserCacheV2 : IUserCache
{
    private readonly ICacheService _cacheService;

    private static readonly string _allUsersKey = "users-all";

    private static readonly string _idUserKey = "users-id-{id}";

    private static readonly string _chatIdUserKey = "users-chat-id-{chatId}";

    public UserCacheV2(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public Task AddOrUpdateAsync(UserDto user, CancellationToken cancellationToken = default)
    {
        return SetWithIdKeysAsync(user, cancellationToken);
    }

    public Task InvalidateAllKeyAsync(CancellationToken cancellationToken = default)
    {
        return _cacheService.DeleteAsync(_allUsersKey, cancellationToken);
    }

    public async Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _cacheService.GetAsync<List<UserDto>>(_allUsersKey, cancellationToken) ?? [];
    }

    public Task<UserDto?> GetByChatIdAsync(long chatId, CancellationToken cancellationToken = default)
    {
        return GetAsync(
            _chatIdUserKey.Replace("{chatId}", chatId.ToString()),
            u => u.ChatId == chatId,
            cancellationToken
        );
    }

    public Task<UserDto?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return GetAsync(
            _idUserKey.Replace("{id}", userId.ToString()),
            u => u.Id == userId,
            cancellationToken
        );
    }

    private async Task<UserDto?> GetAsync(string key, Func<UserDto, bool> predicate, CancellationToken cancellationToken = default)
    {
        var user = await _cacheService.GetAsync<UserDto>(key, cancellationToken);

        if (user is not null)
        {
            return user;
        }

        var users = await GetAllAsync(cancellationToken);

        user = users.FirstOrDefault(predicate);

        if (user is not null)
        {
            await SetWithIdKeysAsync(user, cancellationToken);
        }

        return user;
    }

    private async Task SetWithIdKeysAsync(UserDto user, CancellationToken cancellationToken = default)
    {
        var idKey = _idUserKey.Replace("{id}", user.Id.ToString());
        var chatIdKey = _chatIdUserKey.Replace("{chatId}", user.ChatId.ToString());

        await SetUser(idKey, user, cancellationToken);
        await SetUser(chatIdKey, user, cancellationToken);
    }

    private Task SetUser(string key, UserDto user, CancellationToken cancellationToken = default)
    {
        return _cacheService.SetAsync(key, user, CacheConsts.OneDay, cancellationToken);
    }
}
