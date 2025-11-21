using Eclipse.Application.Contracts.Reminders;
using Eclipse.Application.Contracts.TodoItems;
using Eclipse.Application.Contracts.Users;
using Eclipse.Application.Localizations;
using Eclipse.Common.Caching;
using Eclipse.Core.Context;
using Eclipse.Core.Results;
using Eclipse.Core.Routing;
using Eclipse.Domain.Shared.TodoItems;
using Eclipse.Pipelines.Stores.Messages;

using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.MainMenu.Actions.TodoItems;

[Route("Menu:TodoItems:AddItem", "/todos_add")]
internal sealed class AddTodoItemPipeline : TodoItemsPipelineBase
{
    private readonly ITodoItemService _todoItemService;

    private readonly IUserService _userService;

    private readonly IReminderService _reminderService;

    private readonly ICacheService _cacheService;

    private readonly IMessageStore _messageStore;

    private static readonly string _pipelinePrefix = $"{PipelinePrefix}:AddItem";

    public AddTodoItemPipeline(
        ITodoItemService todoItemService,
        IUserService userService,
        IReminderService reminderService,
        ICacheService cacheService,
        IMessageStore messageStore)
    {
        _todoItemService = todoItemService;
        _userService = userService;
        _reminderService = reminderService;
        _cacheService = cacheService;
        _messageStore = messageStore;
    }

    protected override void Initialize()
    {
        RegisterStage(SendInfo);
        RegisterStage(SaveNewTodoItem);
        RegisterStage(AskToScheduleReminder);
        RegisterStage(ScheduleReminder);
    }

    private async Task<IResult> SendInfo(MessageContext context, CancellationToken cancellationToken)
    {
        var result = await _userService.GetByChatIdAsync(context.ChatId, cancellationToken);

        if (!result.IsSuccess)
        {
            return Text(Localizer.LocalizeError(result.Error));
        }

        var user = result.Value;

        if (user.TodoItems.Count >= TodoItemConstants.Limit)
        {
            FinishPipeline();
            return Text(Localizer[$"{_pipelinePrefix}:{{0}}Limit", user.TodoItems.Count]);
        }

        return Menu(new ReplyKeyboardRemove(), Localizer[$"{_pipelinePrefix}:DescribeWhatToAdd"]);
    }

    private async Task<IResult> SaveNewTodoItem(MessageContext context, CancellationToken cancellationToken)
    {
        if (context.Value.Equals("/cancel"))
        {
            return Menu(TodoItemMenuButtons, Localizer["Okay"]);
        }

        var createNewItemModel = new CreateTodoItemDto
        {
            Text = context.Value,
        };

        var result = await _todoItemService.CreateAsync(context.User.Id, createNewItemModel, cancellationToken);

        if (!result.IsSuccess)
        {
            FinishPipeline();
            return Menu(TodoItemMenuButtons, Localizer.LocalizeError(result.Error));
        }

        await _cacheService.SetAsync(
            $"add-todo-item-{context.ChatId}",
            context.Value,
            new CacheOptions
            {
                Expiration = CacheConsts.ThreeDays,
                Tags = [$"chat-{context.ChatId}"]
            },
            cancellationToken
        );

        var buttons = new InlineKeyboardButton[]
        {
            InlineKeyboardButton.WithCallbackData(Localizer[$"{_pipelinePrefix}:ScheduleReminder"], "true"),
            InlineKeyboardButton.WithCallbackData(Localizer[$"{_pipelinePrefix}:NotScheduleReminder"], "cancel")
        };

        return Menu(buttons, Localizer[$"{_pipelinePrefix}:AskToScheduleReminder"]);
    }

    private async Task<IResult> AskToScheduleReminder(MessageContext context)
    {
        var message = await _messageStore.GetOrDefaultAsync(new MessageKey(context.ChatId));

        if (!context.Value.Equals("true"))
        {
            FinishPipeline();

            List<IResult> actions = message is null
                ? [
                    Menu(TodoItemMenuButtons, Localizer[$"{_pipelinePrefix}:NewItemAdded"])
                ]
                : [
                    Edit(message.Id, InlineKeyboardMarkup.Empty()),
                    Menu(TodoItemMenuButtons, Localizer[$"{_pipelinePrefix}:NewItemAdded"])
                ];

            return Multiple([..actions]);
        }

        return Multiple(message is null
            ? [Text(Localizer[$"{_pipelinePrefix}:AskForReminderTime"])]
            : [
                Edit(message.Id, InlineKeyboardMarkup.Empty()),
                Text(Localizer[$"{_pipelinePrefix}:AskForReminderTime"])
            ]);
    }

    private async Task<IResult> ScheduleReminder(MessageContext context, CancellationToken cancellationToken)
    {
        if (context.Value.Equals("/cancel"))
        {
            return Menu(TodoItemMenuButtons, Localizer[$"{_pipelinePrefix}:NewItemAdded"]);
        }

        if (!context.Value.TryParseAsTimeOnly(out var time))
        {
            RegisterStage(ScheduleReminder);
            return Text(Localizer[$"{_pipelinePrefix}:InvalidTime"]);
        }

        var user = await _userService.GetByChatIdAsync(context.ChatId, cancellationToken);

        if (!user.IsSuccess)
        {
            return Menu(TodoItemMenuButtons, Localizer[$"{_pipelinePrefix}:Error"]);
        }

        string text = await _cacheService.GetOrCreateAsync(
            $"add-todo-item-{context.ChatId}",
            () => Task.FromResult(string.Empty),
            cancellationToken: cancellationToken
        );

        if (text.IsNullOrEmpty())
        {
            return Menu(TodoItemMenuButtons, Localizer[$"{_pipelinePrefix}:Error"]);
        }

        var model = new ReminderCreateDto
        {
            Text = text,
            NotifyAt = time
        };

        await _reminderService.CreateAsync(user.Value.Id, model, cancellationToken);

        return Menu(TodoItemMenuButtons, Localizer[$"{_pipelinePrefix}:ReminderAdded"]);
    }
}
