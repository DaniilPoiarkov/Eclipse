using Eclipse.Application.Contracts.Telegram;
using Eclipse.Common.Results;
using Eclipse.WebAPI.Constants;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Eclipse.WebAPI.Controllers;

[ApiController]
[Route("api/commands")]
[Authorize(Policy = AuthorizationPolicies.Admin)]
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
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _commandService.GetList(cancellationToken));
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] AddCommandRequest request, CancellationToken cancellationToken)
    {
        var result = await _commandService.Add(request, cancellationToken);
        return result.Match(NoContent, error => error.ToProblems(_stringLocalizer));
    }

    [HttpDelete("{command}/remove")]
    public async Task<IActionResult> Remove(string command, CancellationToken cancellationToken)
    {
        await _commandService.Remove(command, cancellationToken);
        return NoContent();
    }
}
