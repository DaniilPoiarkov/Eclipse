using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Infrastructure.Cache;

namespace Eclipse.Application.IdentityUsers;

internal class IdentityUserCache : IIdentityUserCache
{
    private readonly ICacheService _cacheService;

    private static readonly string Key = "Identity-Users";

    public IdentityUserCache(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public void EnsureAdded(IdentityUserDto user)
    {
        var key = new CacheKey(Key);

        var users = _cacheService.GetAndDelete<List<IdentityUserDto>>(key)
            ?? new List<IdentityUserDto>();

        if (users.Exists(u => u.Id == user.Id))
        {
            var cached = users.First(u => u.Id == user.Id);
            users.Remove(cached);
        }

        users.Add(user);

        _cacheService.Set(key, users);
    }

    public IReadOnlyList<IdentityUserDto> GetUsers()
    {
        var key = new CacheKey(Key);

        return _cacheService.Get<List<IdentityUserDto>>(key)
            ?? new List<IdentityUserDto>();
    }
}
