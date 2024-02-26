using Eclipse.Application.Contracts.Telegram.Commands;
using Eclipse.Common.Results;
using Eclipse.WebAPI.Filters.Authorization;

using Microsoft.AspNetCore.Mvc;

namespace Eclipse.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[ApiKeyAuthorize]
public sealed class CommandsController : ControllerBase
{
    private readonly ICommandService _commandService;

    public CommandsController(ICommandService commandService)
    {
        _commandService = commandService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _commandService.GetList(cancellationToken));
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddCommandRequest request, CancellationToken cancellationToken)
    {
        var result = await _commandService.Add(request, cancellationToken);
        return result.ToActionResult(NoContent);
    }

    [HttpDelete("{command}")]
    public async Task<IActionResult> Remove(string command, CancellationToken cancellationToken)
    {
        await _commandService.Remove(command, cancellationToken);
        return NoContent();
    }
}
