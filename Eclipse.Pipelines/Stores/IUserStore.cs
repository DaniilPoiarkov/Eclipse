using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Core.Models;

namespace Eclipse.Pipelines.Stores;

internal interface IUserStore
{
    /// <summary>
    /// Creates new IdentityUser if there is no data associated with specified telegram user, otherwise checks if values are concent
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task AddOrUpdate(TelegramUser user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieve cached users
    /// </summary>
    /// <returns></returns>
    IReadOnlyList<IdentityUserDto> GetCachedUsers();
}
