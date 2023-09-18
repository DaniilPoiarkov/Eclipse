using Eclipse.Application.Contracts.TodoItems;
using Eclipse.Application.Exceptions;
using Eclipse.Core.Attributes;
using Eclipse.Core.Core;

using Serilog;

namespace Eclipse.Pipelines.Pipelines.MainMenu.TodoItems;

[Route("Add item", "/todos_add")]
internal class AddTodoItemPipeline : TodoItemsPipelineBase
{
    private readonly ITodoItemService _todoItemService;

    private readonly ILogger _logger;

    public AddTodoItemPipeline(ITodoItemService todoItemService, ILogger logger)
    {
        _todoItemService = todoItemService;
        _logger = logger;
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
            return Menu(TodoItemMenuButtons, "New item added!");
        }
        catch (EclipseValidationException ex)
        {
            var message = ex.Errors.Select(error => $" - {error}");
            return Menu(TodoItemMenuButtons, $"Some errors occured while creating.{Environment.NewLine}{string.Join(Environment.NewLine, message)}");
        }
        catch (Exception ex)
        {
            _logger.Error("{pipelineName} exception: {error}", nameof(AddTodoItemPipeline), ex.Message);
            return Menu(TodoItemMenuButtons, "Oops, something went wrong. Try again a bit later");
        }
    }
}
