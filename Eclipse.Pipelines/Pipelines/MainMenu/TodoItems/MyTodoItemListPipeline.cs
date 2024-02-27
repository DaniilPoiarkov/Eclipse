using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Application.Contracts.TodoItems;
using Eclipse.Application.Localizations;
using Eclipse.Core.Attributes;
using Eclipse.Core.Core;
using Eclipse.Pipelines.Stores.Messages;

namespace Eclipse.Pipelines.Pipelines.MainMenu.TodoItems;

[Route("Menu:TodoItemsMenu:MyList", "/todos_my")]
internal sealed class MyTodoItemListPipeline : TodoItemsPipelineBase
{
    private readonly ITodoItemService _todoItemService;

    private readonly IIdentityUserService _identityUserService;

    private readonly IMessageStore _messageStore;

    private static readonly string _pipelinePrefix = $"{PipelinePrefix}:MyList";

    private static readonly string _errorMessage = $"{_pipelinePrefix}:Error";

    public MyTodoItemListPipeline(ITodoItemService todoItemService, IIdentityUserService identityUserService, IMessageStore messageStore)
    {
        _todoItemService = todoItemService;
        _identityUserService = identityUserService;
        _messageStore = messageStore;
    }

    protected override void Initialize()
    {
        RegisterStage(SendList);
        RegisterStage(HandleUpdate);
    }

    private async Task<IResult> SendList(MessageContext context, CancellationToken cancellationToken)
    {
        var result = await _identityUserService.GetByChatIdAsync(context.ChatId, cancellationToken);

        if (!result.IsSuccess)
        {
            return Menu(TodoItemMenuButtons, Localizer.LocalizeError(result.Error));
        }

        var items = result.Value.TodoItems;

        if (items.IsNullOrEmpty())
        {
            FinishPipeline();
            return Menu(TodoItemMenuButtons, Localizer[$"{_pipelinePrefix}:Empty"]);
        }

        var message = BuildMessage(items);
        var buttons = BuildButtons(items);

        return Menu(buttons, message);
    }

    private async Task<IResult> HandleUpdate(MessageContext context, CancellationToken cancellationToken)
    {
        var message = _messageStore.GetOrDefault(new MessageKey(context.ChatId));

        if (context.Value.Equals("go_back"))
        {
            return GoBackResult(message);
        }

        var id = context.Value.ToGuid();

        if (id == Guid.Empty)
        {
            return InterruptedResult(message, Localizer[_errorMessage]);
        }

        var result = await _todoItemService.FinishItemAsync(context.ChatId, id, cancellationToken);

        if (!result.IsSuccess)
        {
            return InterruptedResult(message, Localizer[_errorMessage]);
        }

        var items = result.Value.TodoItems;

        if (items.IsNullOrEmpty())
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
}
