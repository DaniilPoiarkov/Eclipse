using Eclipse.Application.Contracts.TodoItems;
using Eclipse.Common.Results;
using Eclipse.Common.Session;

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

    private readonly ICurrentSession _currentSession;

    private readonly IStringLocalizer<TodoItemsController> _stringLocalizer;

    public TodoItemsController(ITodoItemService todoItemService, ICurrentSession currentSession, IStringLocalizer<TodoItemsController> stringLocalizer)
    {
        _todoItemService = todoItemService;
        _currentSession = currentSession;
        _stringLocalizer = stringLocalizer;
    }

    [HttpGet]
    public async Task<IActionResult> GetListAsync(CancellationToken cancellationToken)
    {
        var result = await _todoItemService.GetListAsync(_currentSession.UserId, cancellationToken);
        return result.Match(Ok, error => error.ToProblems(_stringLocalizer));
    }

    [HttpGet("{todoItemId:guid}", Name = "get-todo-item-by-id")]
    public async Task<IActionResult> GetAsync(Guid todoItemId, CancellationToken cancellationToken)
    {
        var result = await _todoItemService.GetAsync(_currentSession.UserId, todoItemId, cancellationToken);

        return result.Match(Ok, error => error.ToProblems(_stringLocalizer));
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddAsync([FromBody] CreateTodoItemDto model, CancellationToken cancellationToken)
    {
        var result = await _todoItemService.CreateAsync(_currentSession.UserId, model, cancellationToken);

        var createdUrl = result.IsSuccess
            ? Url.Link("get-todo-item-by-id", new { todoItemId = result.Value.Id })
            : string.Empty;

        return result.Match(value => Created(createdUrl, value), error => error.ToProblems(_stringLocalizer));
    }

    [HttpPost("{todoItemId:guid}/finish")]
    public async Task<IActionResult> FinishAsync(Guid todoItemId, CancellationToken cancellationToken)
    {
        var result = await _todoItemService.FinishItemAsync(_currentSession.ChatId, todoItemId, cancellationToken);

        return result.Match(NoContent, error => error.ToProblems(_stringLocalizer));
    }
}
