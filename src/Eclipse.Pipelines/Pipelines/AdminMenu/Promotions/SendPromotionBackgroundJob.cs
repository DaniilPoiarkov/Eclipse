using Eclipse.Application.Contracts.Users;
using Eclipse.Common.Background;

using Microsoft.Extensions.Options;

using Telegram.Bot;

namespace Eclipse.Pipelines.Pipelines.AdminMenu.Promotions;

internal sealed class SendPromotionBackgroundJob : IBackgroundJob<SendPromotionBackgroundJobArgs>
{
    private readonly ITelegramBotClient _botClient;

    private readonly IUserService _userService;

    private readonly IOptions<PipelinesOptions> _options;

    public SendPromotionBackgroundJob(ITelegramBotClient botClient, IUserService userService, IOptions<PipelinesOptions> options)
    {
        _botClient = botClient;
        _userService = userService;
        _options = options;
    }

    public async Task ExecuteAsync(SendPromotionBackgroundJobArgs args, CancellationToken cancellationToken = default)
    {
        var users = await _userService.GetAllAsync(cancellationToken);

        var notifications = users.Select(u => SendPromotion(u.ChatId, args, cancellationToken));
        
        var results = await Task.WhenAll(notifications);

        var failedResults = results.Where(r => !r.IsSuccess);

        if (failedResults.IsNullOrEmpty())
        {
            await _botClient.SendMessage(_options.Value.Chat, "Promotion send successfully", cancellationToken: cancellationToken);
            return;
        }

        var errors = failedResults
            .Select((e, i) => $"{i + 1}. Failed to send promotion to chat: {e.ChatId}{Environment.NewLine}{e.Exception}")
            .Select(e => _botClient.SendMessage(_options.Value.Chat, e, cancellationToken: cancellationToken));

        await Task.WhenAll(errors);
    }

    private async Task<SendPromotionResult> SendPromotion(long chatId, SendPromotionBackgroundJobArgs args, CancellationToken cancellationToken)
    {
        try
        {
            await _botClient.CopyMessage(chatId, args.FromChatId, args.MessageId, cancellationToken: cancellationToken);

            return new SendPromotionResult(chatId, true, null);
        }
        catch (Exception ex)
        {
            return new SendPromotionResult(chatId, false, ex);
        }
    }

    record SendPromotionResult(long ChatId, bool IsSuccess, Exception? Exception);
}
