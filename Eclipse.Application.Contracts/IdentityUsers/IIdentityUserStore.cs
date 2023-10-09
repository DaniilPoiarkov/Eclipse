using Eclipse.Core.Models;

namespace Eclipse.Application.Contracts.IdentityUsers;

public interface IIdentityUserStore
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
