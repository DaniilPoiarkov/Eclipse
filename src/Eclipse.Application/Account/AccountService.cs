using Eclipse.Application.Account.SendSignInCode;
using Eclipse.Application.Contracts.Account;
using Eclipse.Common.Background;
using Eclipse.Common.Clock;
using Eclipse.Common.Results;
using Eclipse.Domain.Shared.Errors;
using Eclipse.Domain.Users;

namespace Eclipse.Application.Account;

internal sealed class AccountService : IAccountService
{
    private readonly IUserRepository _userRepository;

    private readonly ITimeProvider _timeProvider;

    private readonly IBackgroundJobManager _backgroundJobManager;

    public AccountService(IUserRepository userRepository, ITimeProvider timeProvider, IBackgroundJobManager backgroundJobManager)
    {
        _userRepository = userRepository;
        _timeProvider = timeProvider;
        _backgroundJobManager = backgroundJobManager;
    }

    public async Task<Result> SendSignInCodeAsync(string userName, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.FindByUserNameAsync(userName, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound<User>();
        }

        user.SetSignInCode(_timeProvider.Now);

        await _userRepository.UpdateAsync(user, cancellationToken);

        await _backgroundJobManager.EnqueueAsync<SendSignInCodeBackgroundJob, SendSignInCodeArgs>(new SendSignInCodeArgs
        {
            ChatId = user.ChatId,
            SignInCode = user.SignInCode,
            Culture = user.Culture
        }, cancellationToken);

        return Result.Success();
    }
}
