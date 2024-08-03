using Eclipse.Application.Contracts.TodoItems;
using Eclipse.Application.Localizations;
using Eclipse.Core.Attributes;
using Eclipse.Core.Core;

namespace Eclipse.Pipelines.Pipelines.MainMenu.TodoItems;

[Route("Menu:TodoItemsMenu:AddItem", "/todos_add")]
internal sealed class AddTodoItemPipeline : TodoItemsPipelineBase
{
    private readonly ITodoItemService _todoItemService;

    private static readonly string _pipelinePrefix = $"{PipelinePrefix}:AddItem";

    public AddTodoItemPipeline(ITodoItemService todoItemService)
    {
        _todoItemService = todoItemService;
    }

    protected override void Initialize()
    {
        RegisterStage(SendInfo);
        RegisterStage(SaveNewTodoItem);
    }

    private IResult SendInfo(MessageContext context)
    {
        return Text(Localizer[$"{_pipelinePrefix}:DiscribeWhatToAdd"]);
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

        var message = result.IsSuccess
            ? Localizer[$"{_pipelinePrefix}:NewItemAdded"]
            : Localizer.LocalizeError(result.Error);

        return Menu(TodoItemMenuButtons, message);
    }
}
