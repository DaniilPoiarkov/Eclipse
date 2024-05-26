using Eclipse.Application.Contracts.Users;
using Eclipse.Common.Results;
using Eclipse.Core.Models;

namespace Eclipse.Pipelines.Users;

internal interface IUserStore
{
    /// <summary>
    /// Creates new User if there is no data associated with specified telegram user, otherwise checks if values are concent
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Result> AddOrUpdateAsync(TelegramUser user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieve cached users
    /// </summary>
    /// <returns></returns>
    Task<IReadOnlyList<UserDto>> GetCachedUsersAsync(CancellationToken cancellationToken = default);
}
