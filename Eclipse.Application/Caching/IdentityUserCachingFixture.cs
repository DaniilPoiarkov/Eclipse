using Eclipse.Application.Contracts.IdentityUsers;

namespace Eclipse.Application.Caching;

internal abstract class IdentityUserCachingFixture
{
    protected readonly IIdentityUserCache UserCache;

    public IdentityUserCachingFixture(IIdentityUserCache userCache)
    {
        UserCache = userCache;
    }

    protected async Task<IdentityUserDto> WithCachingAsync(Func<Task<IdentityUserDto>> action)
    {
        var user = await action();

        UserCache.AddOrUpdate(user);

        return user;
    }
}
