using Eclipse.Common.Results;

namespace Eclipse.Application.Contracts.Authorization;

public interface ILoginManager
{
    Task<Result<LoginResult>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}
