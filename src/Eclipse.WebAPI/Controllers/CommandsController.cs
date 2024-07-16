using Eclipse.Application.Contracts.Telegram.Commands;
using Eclipse.Common.Results;
using Eclipse.WebAPI.Filters.Authorization;

using Microsoft.AspNetCore.Mvc;

namespace Eclipse.WebAPI.Controllers;

[ApiController]
[ApiKeyAuthorize]
[Route("api/commands")]
public sealed class CommandsController : ControllerBase
{
    private readonly ICommandService _commandService;

    public CommandsController(ICommandService commandService)
    {
        _commandService = commandService;
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _commandService.GetList(cancellationToken));
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] AddCommandRequest request, CancellationToken cancellationToken)
    {
        var result = await _commandService.Add(request, cancellationToken);
        return result.Match(NoContent, result.ToProblems);
    }

    [HttpDelete("remove/{command}")]
    public async Task<IActionResult> Remove(string command, CancellationToken cancellationToken)
    {
        await _commandService.Remove(command, cancellationToken);
        return NoContent();
    }
}
