using Eclipse.Application.Contracts.Promotions;
using Eclipse.Common.Background;
using Eclipse.Common.Extensions;
using Eclipse.Domain.Users;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Telegram.Bot;

namespace Eclipse.Application.Promotions;

internal sealed class SendPromotionBackgroundJob : IBackgroundJob<SendPromotionRequest>
{
    private readonly ITelegramBotClient _botClient;

    private readonly IUserRepository _userRepository;

    private readonly ILogger<SendPromotionBackgroundJob> _logger;

    private readonly IOptions<ApplicationOptions> _options;

    public SendPromotionBackgroundJob(
        ITelegramBotClient botClient,
        IUserRepository userRepository,
        ILogger<SendPromotionBackgroundJob> logger,
        IOptions<ApplicationOptions> options)
    {
        _botClient = botClient;
        _userRepository = userRepository;
        _logger = logger;
        _options = options;
    }

    public async Task ExecuteAsync(SendPromotionRequest args, CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetByExpressionAsync(u => u.IsEnabled, cancellationToken);

        await users.Select(u => SendPromotion(u, args, cancellationToken)).WhenAll();
    }

    private async Task SendPromotion(User user, SendPromotionRequest args, CancellationToken cancellationToken)
    {
        try
        {
            await _botClient.CopyMessage(user.ChatId, args.FromChatId, args.MessageId, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            await _botClient.SendMessage(_options.Value.Chat, $"❌ Failed to send promotion to {user.GetReportingDisplayName()}.{Environment.NewLine}{ex}", cancellationToken: cancellationToken);

            _logger.LogError(ex, "Failed to send promotion to {UserId} ({UserName}). Disabling user.", user.Id, user.UserName);

            user.SetIsEnabled(false);
            await _userRepository.UpdateAsync(user, cancellationToken);
        }

        await _botClient.SendMessage(_options.Value.Chat, $"✅ Successfully sent promotion to {user.GetReportingDisplayName()}.", cancellationToken: cancellationToken);
    }
}
