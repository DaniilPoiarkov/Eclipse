using Eclipse.IntegrationEvents.Users;

using MassTransit;

using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Text.Json;

using Telegram.Bot;

namespace Eclipse.Notifications.Users.NewUserRegistered;

public sealed class SendAcknowledgementNotification : IConsumer<NewUserRegisteredIntegrationEvent>
{
    private readonly ILogger<SendAcknowledgementNotification> _logger;

    private readonly ITelegramBotClient _client;

    private readonly IStringLocalizer<SendAcknowledgementNotification> _localizer;

    private readonly IOptions<NotificationsModuleOptions> _options;

    public SendAcknowledgementNotification(
        ILogger<SendAcknowledgementNotification> logger,
        ITelegramBotClient client,
        IStringLocalizer<SendAcknowledgementNotification> localizer,
        IOptions<NotificationsModuleOptions> options)
    {
        _logger = logger;
        _client = client;
        _localizer = localizer;
        _options = options;
    }

    public async Task Consume(ConsumeContext<NewUserRegisteredIntegrationEvent> context)
    {
        _logger.LogInformation("Handler {Handler} with data {Data}", nameof(SendAcknowledgementNotification), JsonSerializer.Serialize(context.Message, new JsonSerializerOptions
        {
            WriteIndented = true,
        }));

        var notification = context.Message;

        var content = _localizer["User:Events:NewUserJoined", notification.UserId, notification.UserName, notification.FirstName, notification.LastName];

        await _client.SendMessage(_options.Value.ChatId, content, cancellationToken: context.CancellationToken);
    }
}
