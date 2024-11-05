using Eclipse.Application.Contracts.Telegram;
using Eclipse.Common.Background;
using Eclipse.Localization.Culture;
using Eclipse.Localization.Extensions;

using Microsoft.Extensions.Localization;

namespace Eclipse.Application.Account.Background;

internal sealed class SendSignInCodeBackgroundJob : IBackgroundJob<SendSignInCodeArgs>
{
    private readonly ICurrentCulture _currentCulture;

    private readonly IStringLocalizer<SendSignInCodeBackgroundJob> _stringLocalizer;

    private readonly ITelegramService _telegramService;

    public SendSignInCodeBackgroundJob(ICurrentCulture currentCulture, IStringLocalizer<SendSignInCodeBackgroundJob> stringLocalizer, ITelegramService telegramService)
    {
        _currentCulture = currentCulture;
        _stringLocalizer = stringLocalizer;
        _telegramService = telegramService;
    }

    public Task ExecuteAsync(SendSignInCodeArgs args, CancellationToken cancellationToken = default)
    {
        using var _ = _currentCulture.UsingCulture(args.Culture);

        _stringLocalizer.UseCurrentCulture(_currentCulture);

        return _telegramService.Send(new SendMessageModel
        {
            ChatId = args.ChatId,
            Message = _stringLocalizer["Account:{0}AuthenticationCode", args.SignInCode]
        }, cancellationToken);
    }
}
