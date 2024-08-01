using Eclipse.Application.Contracts.Account;
using Eclipse.Application.Contracts.Authorization;
using Eclipse.Common.Results;
using Eclipse.WebAPI.Constants;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Eclipse.WebAPI.Controllers;

[ApiController]
[Route("api/account")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    private readonly ILoginManager _loginManager;

    public AccountController(IAccountService accountService, ILoginManager loginManager)
    {
        _accountService = accountService;
        _loginManager = loginManager;
    }

    [HttpPost("send-sign-in-code")]
    [EnableRateLimiting(RateLimiterPolicies.IpAddressFiveMinutes)]
    public async Task<IActionResult> SendSignInCodeAsync([FromQuery] string userName, CancellationToken cancellationToken)
    {
        var result = await _accountService.SendSignInCodeAsync(userName, cancellationToken);
        return result.Match(Ok, result.ToProblems);
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _loginManager.LoginAsync(request, cancellationToken);
        return result.Match(() => Ok(result.Value), result.ToProblems);
    }

    [HttpGet]
    [Authorize]
    public IActionResult Test()
    {
        _ = User;
        return Ok();
    }
}
