using Eclipse.Application.Contracts.Telegram.Messages;
using Eclipse.Application.Contracts.TodoItems;
using Eclipse.Application.Extensions;
using Eclipse.Core.Attributes;
using Eclipse.Core.Core;

namespace Eclipse.Pipelines.Pipelines.MainMenu.TodoItems;

[Route("Menu:TodoItemsMenu:MyList", "/todos_my")]
internal class MyTodoItemListPipeline : TodoItemsPipelineBase
{
    private readonly ITodoItemService _todoItemService;

    private readonly IMessageStore _messageStore;

    private static readonly string _errorMessage = "Pipelines:TodoItems:MyList:Error";

    public MyTodoItemListPipeline(ITodoItemService todoItemService, IMessageStore messageStore)
    {
        _todoItemService = todoItemService;
        _messageStore = messageStore;
    }

    protected override void Initialize()
    {
        RegisterStage(SendList);
        RegisterStage(HandleUpdate);
    }

    private async Task<IResult> SendList(MessageContext context, CancellationToken cancellationToken)
    {
        var items = (await _todoItemService.GetUserItemsAsync(context.User.Id, cancellationToken))
            .Where(item => !item.IsFinished)
            .ToList();

        if (items.Count == 0)
        {
            FinishPipeline();
            return Menu(TodoItemMenuButtons, Localizer["Pipelines:TodoItems:MyList:Empty"]);
        }

        var message = BuildMessage(items);
        var buttons = BuildButtons(items);

        return Menu(buttons, message);
    }

    private async Task<IResult> HandleUpdate(MessageContext context, CancellationToken cancellationToken)
    {
        var message = _messageStore.GetMessage(new MessageKey(context.ChatId));

        if (context.Value.Equals("go_back"))
        {
            return GoBackResult(message);
        }

        var id = context.Value.ToGuid();

        if (id == Guid.Empty)
        {
            return InterruptedResult(message, Localizer[_errorMessage]);
        }

        try
        {
            await _todoItemService.FinishItemAsync(id, cancellationToken);

            var items = await _todoItemService.GetUserItemsAsync(context.ChatId, cancellationToken);

            if (items.Count == 0)
            {
                return AllItemsFinishedResult(message);
            }

            RegisterStage(HandleUpdate);

            if (message is null)
            {
                return await SendList(context, cancellationToken);
            }

            return ItemFinishedResult(items, message);
        }
        catch
        {
            return InterruptedResult(message, Localizer[_errorMessage]);
        }
    }
}
