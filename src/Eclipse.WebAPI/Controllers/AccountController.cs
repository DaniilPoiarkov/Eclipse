using Eclipse.Domain.Users;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Eclipse.WebAPI.Controllers;

[ApiController]
[Route("api/account")]
public class AccountController : ControllerBase
{
    private readonly UserManager _userManager;

    private readonly IConfiguration _configuration;

    public AccountController(UserManager userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    [HttpPost]
    public async Task<IActionResult> LoginAsync(string userName, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByUserNameAsync(userName, cancellationToken);

        if (user is null)
        {
            return BadRequest();
        }

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

        var result = handler.WriteToken(token);

        return Ok(new { Token = result });
    }

    [HttpGet]
    [Authorize]
    public IActionResult Test()
    {
        _ = User;
        return Ok();
    }
}
