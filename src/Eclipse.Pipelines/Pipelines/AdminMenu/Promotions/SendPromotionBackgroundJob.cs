using Eclipse.Application.Contracts.Users;
using Eclipse.Common.Background;
using Eclipse.Common.Extensions;
using Eclipse.Common.Linq;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Telegram.Bot;

namespace Eclipse.Pipelines.Pipelines.AdminMenu.Promotions;

internal sealed class SendPromotionBackgroundJob : IBackgroundJob<SendPromotionBackgroundJobArgs>
{
    private readonly ITelegramBotClient _botClient;

    private readonly IUserService _userService;

    private readonly ILogger<SendPromotionBackgroundJob> _logger;

    private readonly IOptions<PipelinesOptions> _options;

    public SendPromotionBackgroundJob(
        ITelegramBotClient botClient,
        IUserService userService,
        ILogger<SendPromotionBackgroundJob> logger,
        IOptions<PipelinesOptions> options)
    {
        _botClient = botClient;
        _userService = userService;
        _logger = logger;
        _options = options;
    }

    public async Task ExecuteAsync(SendPromotionBackgroundJobArgs args, CancellationToken cancellationToken = default)
    {
        var options = new PaginationRequest<GetUsersRequest>
        {
            Page = 1,
            PageSize = int.MaxValue,
            Options = new GetUsersRequest()
            {
                OnlyActive = true,
            }
        };

        var users = await _userService.GetListAsync(options, cancellationToken);

        await users.Items.Select(u => SendPromotion(u, args, cancellationToken)).WhenAll();
    }

    private async Task SendPromotion(UserSlimDto user, SendPromotionBackgroundJobArgs args, CancellationToken cancellationToken)
    {
        try
        {
            await _botClient.CopyMessage(user.ChatId, args.FromChatId, args.MessageId, cancellationToken: cancellationToken);
            await _botClient.SendMessage(_options.Value.Chat, $"✅ Successfully sent promotion to {user.GetDisplayName()}.", cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            await _botClient.SendMessage(_options.Value.Chat, $"❌ Failed to send promotion to {user.GetDisplayName()}.{Environment.NewLine}{ex}", cancellationToken: cancellationToken);

            _logger.LogError(ex, "Failed to send promotion to {UserId} ({UserName}). Disabling user.", user.Id, user.UserName);

            var options = new UserPartialUpdateDto
            {
                IsEnabled = false,
                IsEnabledChanged = true,
            };

            await _userService.UpdatePartialAsync(user.Id, options, cancellationToken);
        }
    }
}
