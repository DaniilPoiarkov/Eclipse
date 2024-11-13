using Eclipse.Application.Account.Background;
using Eclipse.Application.Contracts.Account;
using Eclipse.Common.Background;
using Eclipse.Common.Clock;
using Eclipse.Common.Results;
using Eclipse.Domain.Shared.Errors;
using Eclipse.Domain.Users;

namespace Eclipse.Application.Account;

internal sealed class AccountService : IAccountService
{
    private readonly UserManager _userManager;

    private readonly ITimeProvider _timeProvider;

    private readonly IBackgroundJobManager _backgroundJobManager;

    public AccountService(UserManager userManager, ITimeProvider timeProvider, IBackgroundJobManager backgroundJobManager)
    {
        _userManager = userManager;
        _timeProvider = timeProvider;
        _backgroundJobManager = backgroundJobManager;
    }

    public async Task<Result> SendSignInCodeAsync(string userName, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByUserNameAsync(userName, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound<User>();
        }

        user.SetSignInCode(_timeProvider.Now);

        await _userManager.UpdateAsync(user, cancellationToken);

        await _backgroundJobManager.EnqueueAsync<SendSignInCodeBackgroundJob, SendSignInCodeArgs>(new SendSignInCodeArgs
        {
            ChatId = user.ChatId,
            SignInCode = user.SignInCode,
            Culture = user.Culture
        }, cancellationToken);

        return Result.Success();
    }
}
