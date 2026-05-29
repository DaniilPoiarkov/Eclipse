using Eclipse.Domain.Shared.Repositories;

namespace Eclipse.Domain.ApiTokens;

public interface IApiTokenRepository : IRepository<ApiToken>
{
    Task<ApiToken?> FindByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ApiToken>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
