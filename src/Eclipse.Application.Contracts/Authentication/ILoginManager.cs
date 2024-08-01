using Eclipse.Common.Results;

namespace Eclipse.Application.Contracts.Authentication;

public interface ILoginManager
{
    Task<Result<LoginResult>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}
