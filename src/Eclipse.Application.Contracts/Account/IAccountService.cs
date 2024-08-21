using Eclipse.Common.Results;

namespace Eclipse.Application.Contracts.Account;

public interface IAccountService
{
    Task<Result> SendSignInCodeAsync(string userName, CancellationToken cancellationToken = default);
}
