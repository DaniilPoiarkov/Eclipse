using Eclipse.Application.Contracts.Account;
using Eclipse.Domain.Users;

using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;

using System.Security.Claims;

using System.Text;

namespace Eclipse.WebAPI.Controllers;

[ApiController]
[Route("api/token")]
public sealed class TokenController : ControllerBase
{
    private readonly UserManager _userManager;

    private readonly IConfiguration _configuration;

    private readonly IAccountService _accountService;

    public TokenController(UserManager userManager, IConfiguration configuration, IAccountService accountService)
    {
        _userManager = userManager;
        _configuration = configuration;
        _accountService = accountService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _accountService.ValidateLoginRequestAsync(request, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.ToProblems();
        }

        var token = await GenerateTokenAsync(request.UserName, cancellationToken);

        return Ok(new
        {
            Token = token,
            Expiration = DateTime.UtcNow.AddMinutes(5).Ticks,
        });
    }

    private async Task<string> GenerateTokenAsync(string userName, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByUserNameAsync(userName, cancellationToken)
            ?? throw new InvalidOperationException("User is null after validating login request.");

        var configuration = _configuration.GetSection("Authorization:JwtBearer");

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Role, user.ChatId == _configuration.GetValue<long>("Telegram:Chat") ? "admin" : "user")
        };

        var key = Encoding.ASCII.GetBytes(configuration["Key"]!);
        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(5),
            IssuedAt = DateTime.UtcNow,
            Issuer = configuration["Issuer"],
            Audience = configuration["Audience"],
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature
            )
        };

        var handler = new JwtSecurityTokenHandler();

        var token = handler.CreateToken(descriptor);

        return handler.WriteToken(token);
    }
}
