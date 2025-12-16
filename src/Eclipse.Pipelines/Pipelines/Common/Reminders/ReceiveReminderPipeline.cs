using Eclipse.Application.Contracts.Reminders;
using Eclipse.Application.Contracts.TodoItems;
using Eclipse.Application.Contracts.Users;
using Eclipse.Common.Caching;
using Eclipse.Core.Context;
using Eclipse.Core.Results;
using Eclipse.Core.Routing;
using Eclipse.Pipelines.Caching;
using Eclipse.Pipelines.Stores.Messages;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using Quartz.Logging;

using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.Common.Reminders;

[Route("", "/href_reminders_receive")]
internal sealed class ReceiveReminderPipeline : EclipsePipelineBase
{
    private readonly IUserService _userService;

    private readonly ITodoItemService _todoItemService;

    private readonly IReminderService _reminderService;

    private readonly IMessageStore _messageStore;

    private readonly ICacheService _cacheService;

    private readonly ILogger<ReceiveReminderPipeline> _logger;

    public ReceiveReminderPipeline(
        IUserService userService,
        ITodoItemService todoItemService,
        IReminderService reminderService,
        IMessageStore messageStore,
        ICacheService cacheService,
        ILogger<ReceiveReminderPipeline> logger)
    {
        _userService = userService;
        _todoItemService = todoItemService;
        _reminderService = reminderService;
        _messageStore = messageStore;
        _cacheService = cacheService;
        _logger = logger;
    }

    protected override void Initialize()
    {
        RegisterStage(SendReminder);
        RegisterStage(FinishTodoItem);
    }

    private async Task<IResult> SendReminder(MessageContext context, CancellationToken cancellationToken)
    {
        // TODO: Remove previous message menu if exists.
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

        return Menu(menu, $"{Localizer["Jobs:SendReminders:RelatedItem"]}\n\r\n\r{payload.Text}");
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
            // TODO: ...
            return Text("KEY IS EMPTY");
        }

        var reply = await _cacheService.GetOrCreateAsync(
            cacheKey,
            () => Task.FromResult<ReceivedReminderReply?>(null),
            cancellationToken: cancellationToken
        );

        if (reply is null)
        {
            // Remove menu from message and process further.
            return Text(Localizer["Error"]);
        }

        if (reply.Action == ReminderReceivedReplyAction.Reschedule)
        {
            // Reschedule reminder.
            return Text("Reschedule");
        }

        if (reply.Action != ReminderReceivedReplyAction.FinishTodoItem)
        {
            // Remove menu from message and process further.
            return Text("NOT FinishTodoItem");
        }

        // TODO: Migrate to use UserId?
        await _todoItemService.FinishAsync(context.ChatId, reply.TodoItemId, cancellationToken);
        await _reminderService.DeleteAsync(reply.UserId, reply.ReminderId, cancellationToken);

        // TODO: Return text as finished.
        return Text("FinishTodoItem");
    }
}
