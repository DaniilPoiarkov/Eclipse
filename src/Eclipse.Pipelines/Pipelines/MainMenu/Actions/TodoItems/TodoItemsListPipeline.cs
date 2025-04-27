using Eclipse.Application.Contracts.TodoItems;
using Eclipse.Application.Contracts.Users;
using Eclipse.Core.Context;
using Eclipse.Core.Pipelines;
using Eclipse.Core.Results;
using Eclipse.Core.Routing;
using Eclipse.Localization.Localizers;
using Eclipse.Pipelines.Stores.Messages;

using System.Globalization;

using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.MainMenu.Actions.TodoItems;

[Route("Menu:TodoItems:List", "/todos_list")]
internal sealed class TodoItemsListPipeline : TodoItemsPipelineBase
{
    private readonly ITodoItemService _todoItemService;

    private readonly IUserService _userService;

    private readonly IMessageStore _messageStore;

    private static readonly string _pipelinePrefix = $"{PipelinePrefix}:List";

    private static readonly string _errorMessage = $"{_pipelinePrefix}:Error";

    public TodoItemsListPipeline(ITodoItemService todoItemService, IUserService userService, IMessageStore messageStore)
    {
        _todoItemService = todoItemService;
        _userService = userService;
        _messageStore = messageStore;
    }

    protected override void Initialize()
    {
        RegisterStage(SendList);
        RegisterStage(HandleUpdate);
    }

    private async Task<IResult> SendList(MessageContext context, CancellationToken cancellationToken)
    {
        var result = await _userService.GetByChatIdAsync(context.ChatId, cancellationToken);

        if (!result.IsSuccess)
        {
            return Menu(TodoItemMenuButtons, Localizer["Error"]);
        }

        var user = result.Value;
        var items = user.TodoItems;

        if (items.IsNullOrEmpty())
        {
            FinishPipeline();
            return Menu(TodoItemMenuButtons, Localizer[$"{_pipelinePrefix}:Empty"]);
        }

        var message = BuildMessage(user.Gmt, CultureInfo.GetCultureInfo(user.Culture), items);
        var buttons = BuildButtons(items);

        return Menu(buttons, message);
    }

    private async Task<IResult> HandleUpdate(MessageContext context, CancellationToken cancellationToken)
    {
        var message = await _messageStore.GetOrDefaultAsync(new MessageKey(context.ChatId), cancellationToken);

        if (context.Value.Equals("go_back"))
        {
            return GoBackResult(message);
        }

        var id = context.Value.ToGuid();

        if (id == Guid.Empty)
        {
            return InvalidActionOrRedirect(context, message);
        }

        var result = await _todoItemService.FinishAsync(context.ChatId, id, cancellationToken);

        if (!result.IsSuccess)
        {
            return InterruptedResult(message, Localizer[_errorMessage]);
        }

        var user = result.Value;
        var items = user.TodoItems;

        if (items.IsNullOrEmpty())
        {
            return AllItemsFinishedResult(message);
        }

        RegisterStage(HandleUpdate);

        if (message is null)
        {
            return await SendList(context, cancellationToken);
        }

        return ItemFinishedResult(user.Gmt, CultureInfo.GetCultureInfo(user.Culture), items, message);
    }

    private IResult InvalidActionOrRedirect(MessageContext context, Message? message)
    {
        try
        {
            var localized = Localizer.ToLocalizableString(context.Value);

            if (!KeyWords.Contains(localized))
            {
                return InterruptedResult(message, Localizer[_errorMessage]);
            }

            return localized switch
            {
                "Menu:TodoItems:List" => RemoveMenuAndRedirect<TodoItemsListPipeline>(message),
                "Menu:TodoItems:AddItem" => RemoveMenuAndRedirect<AddTodoItemPipeline>(message),
                "Menu:MainMenu:Actions" => RemoveMenuAndRedirect<ActionsPipeline>(message),
                _ => InterruptedResult(message, Localizer[_errorMessage]),
            };
        }
        catch
        {
            return InterruptedResult(message, Localizer[_errorMessage]);
        }
    }

    private static IResult RemoveMenuAndRedirect<TPipeline>(Message? message)
        where TPipeline : PipelineBase
    {
        if (message is null)
        {
            return Redirect<TPipeline>();
        }

        return Redirect<TPipeline>(
            Edit(message.MessageId, InlineKeyboardMarkup.Empty())
        );
    }
}
