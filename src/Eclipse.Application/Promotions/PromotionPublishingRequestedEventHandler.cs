using Eclipse.Common.Events;
using Eclipse.Common.Extensions;
using Eclipse.Domain.PromotionLogs;
using Eclipse.Domain.Promotions;
using Eclipse.Domain.Users;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Application.Promotions;

internal sealed class PromotionPublishingRequestedEventHandler : IEventHandler<PromotionPublishingRequestedDomainEvent>
{
    private readonly ITelegramBotClient _botClient;

    private readonly IUserRepository _userRepository;

    private readonly IPromotionRepository _promotionRepository;

    private readonly IPromotionLogRepository _promotionLogRepository;

    private readonly ILogger<PromotionPublishingRequestedEventHandler> _logger;

    private readonly IOptions<ApplicationOptions> _options;

    public PromotionPublishingRequestedEventHandler(
        ITelegramBotClient botClient,
        IUserRepository userRepository,
        IPromotionRepository promotionRepository,
        IPromotionLogRepository promotionLogRepository,
        ILogger<PromotionPublishingRequestedEventHandler> logger,
        IOptions<ApplicationOptions> options)
    {
        _botClient = botClient;
        _userRepository = userRepository;
        _promotionRepository = promotionRepository;
        _promotionLogRepository = promotionLogRepository;
        _logger = logger;
        _options = options;
    }

    public async Task Handle(PromotionPublishingRequestedDomainEvent @event, CancellationToken cancellationToken = default)
    {
        var promotion = await _promotionRepository.FindAsync(@event.PromotionId, cancellationToken);

        if (promotion is null)
        {
            _logger.LogError("Cannot send promotion {Id}, {Reason}.", @event.PromotionId, "Promotion not found");

            await _botClient.SendMessage(
                _options.Value.Chat,
                $"❌ Failed to send promotion {@event.PromotionId}. Promotion not found.",
                cancellationToken: cancellationToken
            );

            return;
        }

        if (!promotion.CanStartPublishing)
        {
            _logger.LogError("Cannot send promotion {Id}, {Reason}.", @event.PromotionId, "Promotion cannot start publishing.");

            await _botClient.SendMessage(
                _options.Value.Chat,
                $"❌ Failed to send promotion {@event.PromotionId}. Promotion cannot start publishing.",
                cancellationToken: cancellationToken
            );

            await _botClient.CopyMessage(_options.Value.Chat, promotion.FromChatId, promotion.MessageId, cancellationToken: cancellationToken);

            return;
        }

        _logger.LogInformation("Start publishing promotion {PromotionId}.", promotion.Id);

        promotion.StartPublishing();
        await _promotionRepository.UpdateAsync(promotion, cancellationToken);

        var users = await _userRepository.GetByExpressionAsync(u => u.IsEnabled, cancellationToken);

        ReplyMarkup replyMarkup = promotion.HasInlineButton
            ? new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl(promotion.InlineButtonText, promotion.InlineButtonLink))
            : InlineKeyboardMarkup.Empty();

        var logs = await users.Select(u => SendPromotion(u, promotion, replyMarkup, cancellationToken)).WhenAll();

        promotion.FinishPublishing();
        
        await _promotionRepository.UpdateAsync(promotion, cancellationToken);
        await _promotionLogRepository.CreateRangeAsync(logs, cancellationToken);

        _logger.LogInformation("Finished publishing promotion {PromotionId}.", promotion.Id);

        await _botClient.SendMessage(_options.Value.Chat,
            $"✅ Finished sending promotion: {promotion.Title}{Environment.NewLine}" +
            $"👤 Users received: {logs.Count(l => l.ReceivedSuccessfully)}{Environment.NewLine}" +
            $"📤 Total receivers: {logs.Count()}",
            cancellationToken: cancellationToken
        );

        foreach (var chunk in logs.Chunk(25))
        {
            await _botClient.SendMessage(_options.Value.Chat,
                chunk.Select(l => l.Message).Join(Environment.NewLine),
                cancellationToken: cancellationToken
            );
        }
    }

    private async Task<PromotionLog> SendPromotion(User user, Promotion promotion, ReplyMarkup replyMarkup, CancellationToken cancellationToken)
    {
        try
        {
            await _botClient.CopyMessage(user.ChatId, promotion.FromChatId, promotion.MessageId, replyMarkup: replyMarkup, cancellationToken: cancellationToken);
            return promotion.AddLog(user.Id, $"✅ Successfully sent promotion to {user.GetReportingDisplayName()}.", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send promotion to {UserId} ({UserName}). Disabling user.", user.Id, user.UserName);

            user.SetIsEnabled(false);
            await _userRepository.UpdateAsync(user, cancellationToken);

            return promotion.AddLog(user.Id, $"❌ Failed to send promotion to {user.GetReportingDisplayName()}. {ex.Message}", false);
        }
    }
}
