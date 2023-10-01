using Eclipse.Core.Models;

namespace Eclipse.Application.Contracts.IdentityUsers;

public interface IIdentityUserStore
{
    Task<IReadOnlyList<IdentityUserDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task EnsureAdded(TelegramUser user, CancellationToken cancellationToken = default);
}
