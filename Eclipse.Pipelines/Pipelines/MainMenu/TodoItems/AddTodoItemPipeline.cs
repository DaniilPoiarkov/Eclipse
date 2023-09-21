using Eclipse.Application.Contracts.TodoItems;
using Eclipse.Application.Exceptions;
using Eclipse.Application.TodoItems.Exceptions;
using Eclipse.Core.Attributes;
using Eclipse.Core.Core;
using Eclipse.Localization.Localizers;

using Serilog;

namespace Eclipse.Pipelines.Pipelines.MainMenu.TodoItems;

[Route("Add item", "/todos_add")]
internal class AddTodoItemPipeline : TodoItemsPipelineBase
{
    private readonly ITodoItemService _todoItemService;

    private readonly ILogger _logger;

    private readonly ILocalizer _localizer;

    public AddTodoItemPipeline(ITodoItemService todoItemService, ILogger logger, ILocalizer localizer)
    {
        _todoItemService = todoItemService;
        _logger = logger;
        _localizer = localizer;
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
        catch(TodoItemLimitException ex)
        {
            return Menu(TodoItemMenuButtons, ex.Message);
        }
        catch (EclipseValidationException ex)
        {
            return Menu(TodoItemMenuButtons, _localizer.FormatLocalizedException(ex));
        }
        catch (Exception ex)
        {
            _logger.Error("{pipelineName} exception: {error}", nameof(AddTodoItemPipeline), ex.Message);
            return Menu(TodoItemMenuButtons, "Oops, something went wrong. Try again a bit later");
        }
    }
}
