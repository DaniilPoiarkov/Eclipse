using Eclipse.Common.Results;

namespace Eclipse.Application.Contracts.Account;

public interface IAccountService
{
    Task<Result> SendSignInCodeAsync(string userName, CancellationToken cancellationToken = default);

    Task<Result> ValidateLoginRequestAsync(LoginRequest request, CancellationToken cancellationToken = default);
}
