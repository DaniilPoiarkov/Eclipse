using Eclipse.Application.Contracts.Telegram.Commands;
using Eclipse.Common.Results;
using Eclipse.WebAPI.Filters.Authorization;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Eclipse.WebAPI.Controllers;

[ApiController]
[ApiKeyAuthorize]
[Route("api/commands")]
public sealed class CommandsController : ControllerBase
{
    private readonly ICommandService _commandService;

    private readonly IStringLocalizer<CommandsController> _stringLocalizer;

    public CommandsController(ICommandService commandService, IStringLocalizer<CommandsController> stringLocalizer)
    {
        _commandService = commandService;
        _stringLocalizer = stringLocalizer;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _commandService.GetList(cancellationToken));
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] AddCommandRequest request, CancellationToken cancellationToken)
    {
        var result = await _commandService.Add(request, cancellationToken);
        return result.Match(NoContent, () => result.ToProblems(_stringLocalizer));
    }

    [HttpDelete("{command}/remove")]
    public async Task<IActionResult> Remove(string command, CancellationToken cancellationToken)
    {
        await _commandService.Remove(command, cancellationToken);
        return NoContent();
    }
}
