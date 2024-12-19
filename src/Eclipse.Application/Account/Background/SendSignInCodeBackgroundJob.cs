using Eclipse.Application.Contracts.Telegram;
using Eclipse.Common.Background;
using Eclipse.Localization.Extensions;

using Microsoft.Extensions.Localization;

namespace Eclipse.Application.Account.Background;

internal sealed class SendSignInCodeBackgroundJob : IBackgroundJob<SendSignInCodeArgs>
{
    private readonly IStringLocalizer<SendSignInCodeBackgroundJob> _stringLocalizer;

    private readonly ITelegramService _telegramService;

    public SendSignInCodeBackgroundJob(IStringLocalizer<SendSignInCodeBackgroundJob> stringLocalizer, ITelegramService telegramService)
    {
        _stringLocalizer = stringLocalizer;
        _telegramService = telegramService;
    }

    public Task ExecuteAsync(SendSignInCodeArgs args, CancellationToken cancellationToken = default)
    {
        using var _ = _stringLocalizer.UsingCulture(args.Culture);

        return _telegramService.Send(new SendMessageModel
        {
            ChatId = args.ChatId,
            Message = _stringLocalizer["Account:{0}AuthenticationCode", args.SignInCode]
        }, cancellationToken);
    }
}
