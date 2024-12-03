using Eclipse.Application.Contracts.TodoItems;
using Eclipse.Application.Contracts.Users;
using Eclipse.Application.Localizations;
using Eclipse.Core.Attributes;
using Eclipse.Core.Core;
using Eclipse.Domain.Shared.TodoItems;

namespace Eclipse.Pipelines.Pipelines.MainMenu.Actions.TodoItems;

[Route("Menu:TodoItems:AddItem", "/todos_add")]
internal sealed class AddTodoItemPipeline : TodoItemsPipelineBase
{
    private readonly ITodoItemService _todoItemService;

    private readonly IUserService _userService;

    private static readonly string _pipelinePrefix = $"{PipelinePrefix}:AddItem";

    public AddTodoItemPipeline(ITodoItemService todoItemService, IUserService userService)
    {
        _todoItemService = todoItemService;
        _userService = userService;
    }

    protected override void Initialize()
    {
        RegisterStage(SendInfo);
        RegisterStage(SaveNewTodoItem);
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

        return Text(Localizer[$"{_pipelinePrefix}:DescribeWhatToAdd"]);
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
