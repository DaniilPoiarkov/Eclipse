using Eclipse.Application.Reminders.Sendings;
using Eclipse.Core.Context;
using Eclipse.Core.Provider;
using Eclipse.Localization.Culture;
using Eclipse.Pipelines.Pipelines;
using Eclipse.Pipelines.Stores.Messages;
using Eclipse.Pipelines.Stores.Pipelines;

using Microsoft.Extensions.Localization;

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

    private readonly IServiceProvider _serviceProvider;

    private readonly IStringLocalizer<ReminderSender> _localizer;

    public HasRelatedItemReminderStrategy(
        ITelegramBotClient client,
        ICurrentCulture currentCulture,
        IMessageStore messageStore,
        IPipelineStore pipelineStore,
        IPipelineProvider pipelineProvider,
        IServiceProvider serviceProvider,
        IStringLocalizer<ReminderSender> localizer)
    {
        _client = client;
        _currentCulture = currentCulture;
        _messageStore = messageStore;
        _pipelineStore = pipelineStore;
        _pipelineProvider = pipelineProvider;
        _serviceProvider = serviceProvider;
        _localizer = localizer;
    }

    public async Task Send(ReminderArguments arguments, CancellationToken cancellationToken = default)
    {
        using var _ = _currentCulture.UsingCulture(arguments.Culture);

        var key = new PipelineKey(arguments.ChatId);
        await _pipelineStore.RemoveAsync(key, cancellationToken);

        var update = new Update
        {
            Message = new Message
            {
                Text = "/href_reminders_receive"
            }
        };

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
            new TelegramUser(arguments.ChatId, string.Empty, string.Empty, string.Empty),
            _serviceProvider
        );

        var result = await pipeline.RunNext(messageContext, cancellationToken);
        var message = await result.SendAsync(_client, cancellationToken);

        await _pipelineStore.SetAsync(key, pipeline, cancellationToken);

        if (message is not null)
        {
            await _messageStore.SetAsync(new MessageKey(arguments.ChatId), message, cancellationToken);
        }
    }
}
