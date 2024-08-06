using Eclipse.Application.Contracts.TodoItems;
using Eclipse.Common.Results;
using Eclipse.Common.Session;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eclipse.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/todo-items")]
public class TodoItemsController : ControllerBase
{
    private readonly ITodoItemService _todoItemService;

    private readonly ICurrentSession _currentSession;

    public TodoItemsController(ITodoItemService todoItemService, ICurrentSession currentSession)
    {
        _todoItemService = todoItemService;
        _currentSession = currentSession;
    }

    [HttpGet]
    public async Task<IActionResult> GetListAsync(CancellationToken cancellationToken)
    {
        if (!_currentSession.UserId.HasValue)
        {
            return Unauthorized();
        }

        var result = await _todoItemService.GetListAsync(_currentSession.UserId.Value, cancellationToken);
        return result.Match(() => Ok(result.Value), result.ToProblems);
    }

    [HttpGet("{todoItemId:guid}", Name = "get-todo-item-by-id")]
    public async Task<IActionResult> GetAsync(Guid todoItemId, CancellationToken cancellationToken)
    {
        if (!_currentSession.UserId.HasValue)
        {
            return Unauthorized();
        }

        var result = await _todoItemService.GetAsync(_currentSession.UserId.Value, todoItemId, cancellationToken);

        return result.Match(() => Ok(result.Value), result.ToProblems);
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddAsync([FromBody] CreateTodoItemDto model, CancellationToken cancellationToken)
    {
        if (!_currentSession.UserId.HasValue)
        {
            return Unauthorized();
        }

        var result = await _todoItemService.CreateAsync(_currentSession.UserId.Value, model, cancellationToken);

        var createdUrl = result.IsSuccess
            ? Url.Link("get-todo-item-by-id", new { todoItemId = result.Value.Id })
            : string.Empty;

        return result.Match(() => Created(createdUrl, result.Value), result.ToProblems);
    }

    [HttpPost("{todoItemId:guid}/finish")]
    public async Task<IActionResult> FinishAsync(Guid todoItemId, CancellationToken cancellationToken)
    {
        if (!_currentSession.ChatId.HasValue)
        {
            return Unauthorized();
        }

        var result = await _todoItemService.FinishItemAsync(_currentSession.ChatId.Value, todoItemId, cancellationToken);

        return result.Match(NoContent, result.ToProblems);
    }
}
