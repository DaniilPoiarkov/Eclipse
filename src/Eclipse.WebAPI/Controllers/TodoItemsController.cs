using Eclipse.Application.Contracts.TodoItems;
using Eclipse.Common.Results;
using Eclipse.WebAPI.Extensions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Eclipse.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/todo-items")]
public sealed class TodoItemsController : ControllerBase
{
    private readonly ITodoItemService _todoItemService;

    private readonly IStringLocalizer<TodoItemsController> _stringLocalizer;

    public TodoItemsController(ITodoItemService todoItemService, IStringLocalizer<TodoItemsController> stringLocalizer)
    {
        _todoItemService = todoItemService;
        _stringLocalizer = stringLocalizer;
    }

    [HttpGet]
    public async Task<IActionResult> GetListAsync(CancellationToken cancellationToken)
    {
        var result = await _todoItemService.GetListAsync(User.GetUserId(), cancellationToken);
        return result.Match(Ok, error => error.ToProblems(_stringLocalizer));
    }

    [HttpGet("{todoItemId:guid}", Name = "get-todo-item-by-id")]
    public async Task<IActionResult> GetAsync(Guid todoItemId, CancellationToken cancellationToken)
    {
        var result = await _todoItemService.GetAsync(User.GetUserId(), todoItemId, cancellationToken);

        return result.Match(Ok, error => error.ToProblems(_stringLocalizer));
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddAsync([FromBody] CreateTodoItemDto model, CancellationToken cancellationToken)
    {
        var result = await _todoItemService.CreateAsync(User.GetUserId(), model, cancellationToken);

        var createdUrl = result.IsSuccess
            ? Url.Link("get-todo-item-by-id", new { todoItemId = result.Value.Id })
            : string.Empty;

        return result.Match(value => Created(createdUrl, value), error => error.ToProblems(_stringLocalizer));
    }

    [HttpPost("{todoItemId:guid}/finish")]
    public async Task<IActionResult> FinishAsync(Guid todoItemId, CancellationToken cancellationToken)
    {
        var result = await _todoItemService.FinishItemAsync(User.GetChatId(), todoItemId, cancellationToken);

        return result.Match(NoContent, error => error.ToProblems(_stringLocalizer));
    }
}
