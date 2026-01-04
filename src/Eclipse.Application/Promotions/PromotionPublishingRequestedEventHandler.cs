using Eclipse.Common.Events;
using Eclipse.Common.Extensions;
using Eclipse.Domain.Promotions;
using Eclipse.Domain.Users;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Telegram.Bot;

namespace Eclipse.Application.Promotions;

internal sealed class PromotionPublishingRequestedEventHandler : IEventHandler<PromotionPublishingRequestedDomainEvent>
{
    private readonly ITelegramBotClient _botClient;

    private readonly IUserRepository _userRepository;

    private readonly IPromotionRepository _promotionRepository;

    private readonly ILogger<PromotionPublishingRequestedEventHandler> _logger;

    private readonly IOptions<ApplicationOptions> _options;

    public PromotionPublishingRequestedEventHandler(
        ITelegramBotClient botClient,
        IUserRepository userRepository,
        IPromotionRepository promotionRepository,
        ILogger<PromotionPublishingRequestedEventHandler> logger,
        IOptions<ApplicationOptions> options)
    {
        _botClient = botClient;
        _userRepository = userRepository;
        _promotionRepository = promotionRepository;
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
                $"❌ Failed to send promotion {@event.PromotionId}.{Environment.NewLine}Promotion not found.",
                cancellationToken: cancellationToken
            );

            return;
        }

        if (!promotion.CanStartPublishing)
        {
            _logger.LogError("Cannot send promotion {Id}, {Reason}.", @event.PromotionId, "Promotion cannot start publishing.");

            await _botClient.SendMessage(
                _options.Value.Chat,
                $"❌ Failed to send promotion {@event.PromotionId}.{Environment.NewLine}Promotion cannot start publishing.",
                cancellationToken: cancellationToken
            );

            await _botClient.CopyMessage(_options.Value.Chat, promotion.FromChatId, promotion.MessageId, cancellationToken: cancellationToken);

            return;
        }

        _logger.LogInformation("Start publishing promotion {PromotionId}.", promotion.Id);

        promotion.StartPublishing();
        await _promotionRepository.UpdateAsync(promotion, cancellationToken);

        var users = await _userRepository.GetByExpressionAsync(u => u.IsEnabled, cancellationToken);

        await users.Select(u => SendPromotion(u, promotion.FromChatId, promotion.MessageId, cancellationToken)).WhenAll();

        promotion.FinishPublishing();
        await _promotionRepository.UpdateAsync(promotion, cancellationToken);

        _logger.LogInformation("Finished publishing promotion {PromotionId}.", promotion.Id);
    }

    private async Task SendPromotion(User user, long fromChatId, int messageId, CancellationToken cancellationToken)
    {
        try
        {
            await _botClient.CopyMessage(user.ChatId, fromChatId, messageId, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            await _botClient.SendMessage(_options.Value.Chat, $"❌ Failed to send promotion to {user.GetReportingDisplayName()}.{Environment.NewLine}{ex}", cancellationToken: cancellationToken);

            _logger.LogError(ex, "Failed to send promotion to {UserId} ({UserName}). Disabling user.", user.Id, user.UserName);

            user.SetIsEnabled(false);
            await _userRepository.UpdateAsync(user, cancellationToken);
            return;
        }

        await _botClient.SendMessage(_options.Value.Chat, $"✅ Successfully sent promotion to {user.GetReportingDisplayName()}.", cancellationToken: cancellationToken);
    }
}
