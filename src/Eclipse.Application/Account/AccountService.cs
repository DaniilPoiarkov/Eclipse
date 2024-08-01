using Eclipse.Application.Contracts.Account;
using Eclipse.Application.Contracts.Telegram;
using Eclipse.Common.Results;
using Eclipse.Domain.Shared.Errors;
using Eclipse.Domain.Users;
using Eclipse.Localization.Culture;
using Eclipse.Localization.Extensions;

using Microsoft.Extensions.Localization;

namespace Eclipse.Application.Account;

internal sealed class AccountService : IAccountService
{
    private readonly UserManager _userManager;

    private readonly ITelegramService _telegramService;

    private readonly IStringLocalizer<AccountService> _stringLocalizer;

    private readonly ICurrentCulture _currentCulture;

    public AccountService(UserManager userManager, ITelegramService telegramService, IStringLocalizer<AccountService> stringLocalizer, ICurrentCulture currentCulture)
    {
        _userManager = userManager;
        _telegramService = telegramService;
        _stringLocalizer = stringLocalizer;
        _currentCulture = currentCulture;
    }

    public async Task<Result> SendSignInCodeAsync(string userName, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByUserNameAsync(userName, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound(typeof(User));
        }

        using var _ = _currentCulture.UsingCulture(user.Culture);
        _stringLocalizer.UseCurrentCulture(_currentCulture);

        user.SetSignInCode();

        await _userManager.UpdateAsync(user, cancellationToken);

        return await _telegramService.Send(new SendMessageModel
        {
            ChatId = user.ChatId,
            Message = _stringLocalizer["Account:{0}AuthenticationCode", user.SignInCode ?? string.Empty]
        }, cancellationToken);
    }
}
