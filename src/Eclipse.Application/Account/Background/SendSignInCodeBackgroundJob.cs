using Eclipse.Application.Contracts.Telegram;
using Eclipse.Common.Background;
using Eclipse.Localization.Culture;

using Microsoft.Extensions.Localization;

namespace Eclipse.Application.Account.Background;

internal sealed class SendSignInCodeBackgroundJob : IBackgroundJob<SendSignInCodeArgs>
{
    private readonly IStringLocalizer<SendSignInCodeBackgroundJob> _stringLocalizer;

    private readonly ITelegramService _telegramService;

    private readonly ICurrentCulture _currentCulture;

    public SendSignInCodeBackgroundJob(
        IStringLocalizer<SendSignInCodeBackgroundJob> stringLocalizer,
        ITelegramService telegramService,
        ICurrentCulture currentCulture)
    {
        _stringLocalizer = stringLocalizer;
        _telegramService = telegramService;
        _currentCulture = currentCulture;
    }

    public Task ExecuteAsync(SendSignInCodeArgs args, CancellationToken cancellationToken = default)
    {
        using var _ = _currentCulture.UsingCulture(args.Culture);

        return _telegramService.Send(new SendMessageModel
        {
            ChatId = args.ChatId,
            Message = _stringLocalizer["Account:{0}AuthenticationCode", args.SignInCode]
        }, cancellationToken);
    }
}
