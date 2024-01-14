using Eclipse.Application.Contracts.TodoItems;
using Eclipse.Core.Attributes;
using Eclipse.Core.Core;
using Eclipse.Domain.Exceptions;
using Eclipse.Infrastructure.Exceptions;
using Eclipse.Localization.Exceptions;

using Serilog;

namespace Eclipse.Pipelines.Pipelines.MainMenu.TodoItems;

[Route("Menu:TodoItemsMenu:AddItem", "/todos_add")]
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

    private IResult SendInfo(MessageContext context)
    {
        return Text(Localizer["Pipelines:TodoItems:AddItem:DiscribeWhatToAdd"]);
    }

    private async Task<IResult> SaveNewTodoItem(MessageContext context, CancellationToken cancellationToken)
    {
        var createNewItemModel = new CreateTodoItemDto
        {
            Text = context.Value,
            UserId = context.User.Id,
        };

        try
        {
            await _todoItemService.CreateAsync(createNewItemModel, cancellationToken);
            return Menu(TodoItemMenuButtons, Localizer["Pipelines:TodoItems:AddItem:NewItemAdded"]);
        }
        catch (EclipseValidationException ex)
        {
            var template = Localizer[ex.Message];
            var message = string.Format(template, [..ex.Data.Values]);

            return Menu(TodoItemMenuButtons, message);
        }
        catch (Exception ex)
        {
            _logger.Error("{pipelineName} exception: {error}", nameof(AddTodoItemPipeline), ex);
            return Menu(TodoItemMenuButtons, Localizer["Pipelines:TodoItems:AddItem:Error"]);
        }
    }
}
