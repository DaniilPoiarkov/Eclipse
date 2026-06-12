using Eclipse.Common.Results;

using System.Security.Claims;

namespace Eclipse.Application.Contracts.ApiTokens;

public interface IApiTokenService
{
    Task<Result<CreateApiTokenResult>> CreateAsync(Guid userId, CreateApiTokenDto dto, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<ApiTokenDto>>> GetListAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<Result> RevokeAsync(Guid userId, Guid tokenId, CancellationToken cancellationToken = default);

    Task<Result> RevokeByNameAsync(Guid userId, string name, CancellationToken cancellationToken = default);

    Task<ClaimsPrincipal?> AuthenticateAsync(string tokenHash, CancellationToken cancellationToken = default);
}
