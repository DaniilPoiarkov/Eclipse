using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Common.Results;

namespace Eclipse.Application.Caching;

internal abstract class IdentityUserCachingFixture
{
    protected readonly IIdentityUserCache UserCache;

    public IdentityUserCachingFixture(IIdentityUserCache userCache)
    {
        UserCache = userCache;
    }

    protected async Task<Result<IdentityUserDto>> WithCachingAsync(Func<Task<Result<IdentityUserDto>>> action, CancellationToken cancellationToken = default)
    {
        var result = await action();

        if (result.IsSuccess)
        {
            await UserCache.AddOrUpdateAsync(result.Value, cancellationToken);
        }

        return result;
    }
}
