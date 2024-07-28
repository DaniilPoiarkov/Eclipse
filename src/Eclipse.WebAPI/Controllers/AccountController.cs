using Eclipse.Application.Contracts.Account;
using Eclipse.Common.Results;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eclipse.WebAPI.Controllers;

[ApiController]
[Route("api/account")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpPost("send-sign-in-code")]
    public async Task<IActionResult> SendSignInCodeAsync([FromQuery] string userName, CancellationToken cancellationToken)
    {
        var result = await _accountService.SendSignInCodeAsync(userName, cancellationToken);
        return result.Match(Ok, result.ToProblems);
    }

    [HttpGet]
    [Authorize]
    public IActionResult Test()
    {
        _ = User;
        return Ok();
    }
}
