using Eclipse.Application.Contracts.Account;
using Eclipse.Application.Contracts.Authorization;
using Eclipse.Common.Results;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Eclipse.WebAPI.Controllers;

[ApiController]
[Route("api/account")]
public sealed class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    private readonly ILoginManager _loginManager;

    private readonly IStringLocalizer<AccountController> _stringLocalizer;

    public AccountController(
        IAccountService accountService,
        ILoginManager loginManager,
        IStringLocalizer<AccountController> stringLocalizer)
    {
        _accountService = accountService;
        _loginManager = loginManager;
        _stringLocalizer = stringLocalizer;
    }

    [HttpPost("send-sign-in-code")]
    public async Task<IActionResult> SendSignInCodeAsync([FromQuery] string userName, CancellationToken cancellationToken)
    {
        var result = await _accountService.SendSignInCodeAsync(userName, cancellationToken);
        return result.Match(Ok, error => error.ToProblems(_stringLocalizer));
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _loginManager.LoginAsync(request, cancellationToken);
        return result.Match(Ok, error => error.ToProblems(_stringLocalizer));
    }
}
