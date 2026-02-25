using Eclipse.Application.Contracts.Users;
using Eclipse.Application.Reminders.Sendings;
using Eclipse.Common.Caching;
using Eclipse.Core.Context;
using Eclipse.Core.Provider;
using Eclipse.Core.Stores;
using Eclipse.Localization.Culture;
using Eclipse.Pipelines.Caching;
using Eclipse.Pipelines.Pipelines;
using Eclipse.Pipelines.Pipelines.Common.Reminders;

using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Eclipse.Pipelines.Reminders;

internal sealed class HasRelatedItemReminderStrategy : IReminderSenderStrategy
{
    private readonly ITelegramBotClient _client;

    private readonly ICurrentCulture _currentCulture;

    private readonly IMessageStore _messageStore;

    private readonly IPipelineStore _pipelineStore;

    private readonly IPipelineProvider _pipelineProvider;

    private readonly IUserService _userService;

    private readonly ICacheService _cacheService;

    private readonly IServiceProvider _serviceProvider;

    private readonly IStringLocalizer<ReminderSender> _localizer;

    private readonly ILogger<HasRelatedItemReminderStrategy> _logger;

    public HasRelatedItemReminderStrategy(
        ITelegramBotClient client,
        ICurrentCulture currentCulture,
        IMessageStore messageStore,
        IPipelineStore pipelineStore,
        IPipelineProvider pipelineProvider,
        IUserService userService,
        ICacheService cacheService,
        IServiceProvider serviceProvider,
        IStringLocalizer<ReminderSender> localizer,
        ILogger<HasRelatedItemReminderStrategy> logger)
    {
        _client = client;
        _currentCulture = currentCulture;
        _messageStore = messageStore;
        _pipelineStore = pipelineStore;
        _pipelineProvider = pipelineProvider;
        _userService = userService;
        _cacheService = cacheService;
        _serviceProvider = serviceProvider;
        _localizer = localizer;
        _logger = logger;
    }

    public async Task Send(ReminderArguments arguments, CancellationToken cancellationToken = default)
    {
        using var _ = _currentCulture.UsingCulture(arguments.Culture);
        var user = await _userService.GetByIdAsync(arguments.UserId, cancellationToken);

        if (!user.IsSuccess)
        {
            _logger.LogError("Cannot send reminder with related todo item. User not found.");
            return;
        }

        var update = new Update
        {
            Message = new Message
            {
                Text = "/href_reminders_receive",
                From = new User
                {
                    Id = user.Value.ChatId,
                    FirstName = user.Value.Name,
                    LastName = user.Value.Surname,
                    Username = user.Value.UserName,
                    LanguageCode = user.Value.Culture
                }
            }
        };

        await _pipelineStore.RemoveAll(user.Value.ChatId, cancellationToken);

        // TODO: Try to get pipeline directly. Should be able to inject.
        var pipeline = (EclipsePipelineBase)_pipelineProvider.Get(update);

        pipeline.SetLocalizer(_localizer);

        var payload = $$"""
            {
                "ReminderId": "{{arguments.ReminderId}}",
                "TodoItemId": "{{arguments.RelatedItemId}}",
                "UserId": "{{arguments.UserId}}",
                "Text": "{{arguments.Text}}"
            }
            """;

        var messageContext = new MessageContext(
            arguments.ChatId,
            payload,
            new TelegramUser(arguments.ChatId, user.Value.Name, user.Value.Surname, user.Value.UserName),
            _serviceProvider
        );

        var result = await pipeline.RunNext(messageContext, cancellationToken);
        var message = await result.SendAsync(_client, cancellationToken);

        if (message is null)
        {
            _logger.LogWarning("{Pipeline} pipeline was initiated successfully, but sent message is null.", nameof(ReceiveReminderPipeline));
            await _pipelineStore.Set(user.Value.ChatId, pipeline, cancellationToken);
            return;
        }

        await _pipelineStore.Set(user.Value.ChatId, message.Id, pipeline, cancellationToken);
        await _messageStore.Set(arguments.ChatId, message, cancellationToken);

        // Needed to remove menu for action step.
        await _cacheService.SetForThreeDaysAsync(
            $"users-{arguments.UserId}-reminders-{arguments.ReminderId}-receive-message",
            message,
            arguments.ChatId,
            cancellationToken
        );
    }
}
