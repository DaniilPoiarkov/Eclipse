using Eclipse.Application.Contracts.Users;
using Eclipse.Common.Results;

namespace Eclipse.Application.Caching;

internal abstract class UserCachingFixture
{
    protected readonly IUserCache UserCache;

    public UserCachingFixture(IUserCache userCache)
    {
        UserCache = userCache;
    }

    protected async Task<Result<UserDto>> WithCachingAsync(Func<Task<Result<UserDto>>> action, CancellationToken cancellationToken = default)
    {
        var result = await action();

        if (result.IsSuccess)
        {
            await UserCache.AddOrUpdateAsync(result.Value, cancellationToken);
        }

        return result;
    }
}
