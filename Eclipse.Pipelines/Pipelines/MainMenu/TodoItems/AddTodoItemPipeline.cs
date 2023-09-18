using Eclipse.Application.Contracts.TodoItems;
using Eclipse.Core.Attributes;
using Eclipse.Core.Core;

namespace Eclipse.Pipelines.Pipelines.MainMenu.TodoItems;

[Route("Add item", "/todos_add")]
internal class AddTodoItemPipeline : EclipsePipelineBase
{
    private readonly ITodoItemService _todoItemService;

    public AddTodoItemPipeline(ITodoItemService todoItemService)
    {
        _todoItemService = todoItemService;
    }

    protected override void Initialize()
    {
        RegisterStage(SendInfo);
        RegisterStage(SaveNewTodoItem);
    }

    private static IResult SendInfo(MessageContext context)
    {
        return Text("Describe what do you wanna to add");
    }

    private IResult SaveNewTodoItem(MessageContext context)
    {
        var createNewItemModel = new CreateTodoItemDto
        {
            Text = context.Value,
            UserId = context.User.Id,
        };

        try
        {
            _todoItemService.AddItem(createNewItemModel);
            return Menu(MainMenuButtons, "New item added!");
        }
        catch (Exception ex)
        {
            return Menu(MainMenuButtons, ex.Message);
        }
    }
}
