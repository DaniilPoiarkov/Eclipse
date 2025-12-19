using Eclipse.Application.Contracts.Reminders;
using Eclipse.Application.Contracts.TodoItems;
using Eclipse.Common.Caching;
using Eclipse.Common.Clock;
using Eclipse.Core.Context;
using Eclipse.Core.Results;
using Eclipse.Core.Routing;
using Eclipse.Pipelines.Caching;
using Eclipse.Pipelines.Stores.Messages;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.Common.Reminders;

[Route("", "/href_reminders_receive")]
internal sealed class ReceiveReminderPipeline : EclipsePipelineBase
{
    private readonly ITodoItemService _todoItemService;

    private readonly IReminderService _reminderService;

    private readonly IMessageStore _messageStore;

    private readonly ICacheService _cacheService;

    private readonly ITimeProvider _timeProvider;

    private readonly ILogger<ReceiveReminderPipeline> _logger;

    public ReceiveReminderPipeline(
        ITodoItemService todoItemService,
        IReminderService reminderService,
        IMessageStore messageStore,
        ICacheService cacheService,
        ITimeProvider timeProvider,
        ILogger<ReceiveReminderPipeline> logger)
    {
        _todoItemService = todoItemService;
        _reminderService = reminderService;
        _messageStore = messageStore;
        _cacheService = cacheService;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    protected override void Initialize()
    {
        RegisterStage(SendReminder);
        RegisterStage(FinishTodoItem);
    }

    private async Task<IResult> SendReminder(MessageContext context, CancellationToken cancellationToken)
    {
        var message = await _messageStore.GetOrDefaultAsync(
            new MessageKey(context.ChatId),
            cancellationToken
        );

        var payload = JsonConvert.DeserializeObject<ReminderReceivedPayload>(context.Value);

        if (payload is null)
        {
            _logger.LogError("Cannot deserialize ReminderReceivedPayload from {Payload}", context.Value);

            FinishPipeline();
            return Empty();
        }

        var finish = new ReceivedReminderReply(
            payload.ReminderId,
            payload.TodoItemId,
            payload.UserId,
            ReminderReceivedReplyAction.FinishTodoItem
        );

        var reschedule = new ReceivedReminderReply(
            payload.ReminderId,
            payload.TodoItemId,
            payload.UserId,
            ReminderReceivedReplyAction.Reschedule
        );

        var finishKey = $"pipelines-reminders-receive-finish-{payload.UserId}-{payload.ReminderId}";
        var rescheduleKey = $"pipelines-reminders-receive-reschedule-{payload.UserId}-{payload.ReminderId}";

        await _cacheService.SetForThreeDaysAsync(finishKey, finish, context.ChatId, cancellationToken);
        await _cacheService.SetForThreeDaysAsync(rescheduleKey, reschedule, context.ChatId, cancellationToken);

        var finishKeyPayload = Guid.CreateVersion7().ToString();
        var rescheduleKeyPayload = Guid.CreateVersion7().ToString();

        await _cacheService.SetForThreeDaysAsync(finishKeyPayload, finishKey, context.ChatId, cancellationToken);
        await _cacheService.SetForThreeDaysAsync(rescheduleKeyPayload, rescheduleKey, context.ChatId, cancellationToken);

        List<InlineKeyboardButton> menu = [
            InlineKeyboardButton.WithCallbackData(Localizer["Pipelines:Reminders:Receive:Finish"], finishKeyPayload),
            InlineKeyboardButton.WithCallbackData(Localizer["Pipelines:Reminders:Receive:Reschedule"], rescheduleKeyPayload)
        ];

        return RemoveInlineMenuAndSend(menu, $"{Localizer["Jobs:SendReminders:RelatedItem"]}\n\r\n\r{payload.Text}", message);
    }

    private async Task<IResult> FinishTodoItem(MessageContext context, CancellationToken cancellationToken)
    {
        var message = await _messageStore.GetOrDefaultAsync(
            new MessageKey(context.ChatId),
            cancellationToken
        );

        var cacheKey = await _cacheService.GetOrCreateAsync(
            context.Value,
            () => Task.FromResult(string.Empty),
            cancellationToken: cancellationToken
        );

        if (cacheKey.IsNullOrEmpty())
        {
            return RemoveInlineMenuAndSend(Localizer["Error"], message);
        }

        var reply = await _cacheService.GetOrCreateAsync(
            cacheKey,
            () => Task.FromResult<ReceivedReminderReply?>(null),
            cancellationToken: cancellationToken
        );

        if (reply is null)
        {
            return RemoveInlineMenuAndSend(Localizer["Error"], message);
        }

        if (reply.Action == ReminderReceivedReplyAction.Reschedule)
        {
            await _cacheService.SetForThreeDaysAsync(
                $"pipelines-receive-reminder-reschedule-{context.ChatId}",
                reply,
                context.ChatId,
                cancellationToken
            );

            RegisterStage(RescheduleReminder);
            return RemoveInlineMenuAndSend(Localizer["Pipelines:Reminders:Receive:RescheduleReminder"], message);
        }

        if (reply.Action != ReminderReceivedReplyAction.FinishTodoItem)
        {
            await _reminderService.DeleteAsync(reply.UserId, reply.ReminderId, cancellationToken);
            return RemoveInlineMenuAndSend(Localizer["Pipelines:Reminders:Receive:InvalidAction"], message);
        }

        await _todoItemService.FinishAsync(context.ChatId, reply.TodoItemId, cancellationToken);
        await _reminderService.DeleteAsync(reply.UserId, reply.ReminderId, cancellationToken);

        return RemoveInlineMenuAndSend(Localizer["Pipelines:Reminders:Receive:TodoItemFinished"], message);
    }

    private async Task<IResult> RescheduleReminder(MessageContext context, CancellationToken cancellationToken)
    {
        var payload = await _cacheService.GetOrCreateAsync(
            $"pipelines-receive-reminder-reschedule-{context.ChatId}",
            () => Task.FromResult<ReceivedReminderReply?>(null),
            cancellationToken: cancellationToken
        );

        if (payload is null)
        {
            return Text(Localizer["Error"]);
        }

        if (context.Value.Equals("/cancel"))
        {
            return Menu(MainMenuButtons, Localizer["Pipelines:Reminders:Receive:RescheduleReminder:Canceled"]);
        }

        if (!context.Value.TryParseAsTimeOnly(out var time))
        {
            RegisterStage(RescheduleReminder);
            return Text(Localizer["Pipelines:Reminders:Receive:RescheduleReminder:InvalidTime"]);
        }

        await _reminderService.RescheduleAsync(
            payload.UserId,
            payload.ReminderId,
            new RescheduleReminderOptions(true, time),
            cancellationToken
        );

        return Text(Localizer["Pipelines:Reminders:Receive:RescheduleReminder:Rescheduled"]);
    }
}
