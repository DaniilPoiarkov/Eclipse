using Eclipse.Core.Models;

namespace Eclipse.Application.Contracts.IdentityUsers;

public interface IIdentityUserStore
{
    Task EnsureAdded(TelegramUser user, CancellationToken cancellationToken = default);

    IReadOnlyList<IdentityUserDto> GetCachedUsers();
}
