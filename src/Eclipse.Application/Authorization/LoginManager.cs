using Eclipse.Application.Contracts.Authorization;
using Eclipse.Common.Clock;
using Eclipse.Common.Results;
using Eclipse.Domain.Shared.Errors;
using Eclipse.Domain.Shared.Identity;
using Eclipse.Domain.Users;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Eclipse.Application.Authorization;

internal sealed class LoginManager : ILoginManager
{
    private readonly UserManager _userManager;

    private readonly IConfiguration _configuration;

    private readonly IUserClaimsPrincipalFactory<User> _userClaimsPrincipalFactory;

    private readonly ITimeProvider _timeProvider;

    public LoginManager(
        UserManager userManager,
        IConfiguration configuration,
        IUserClaimsPrincipalFactory<User> userClaimsPrincipalFactory,
        ITimeProvider timeProvider)
    {
        _userManager = userManager;
        _configuration = configuration;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        _timeProvider = timeProvider;
    }

    public async Task<Result<LoginResult>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByUserNameAsync(request.UserName, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound(typeof(User));
        }

        if (!user.IsValidSignInCode(_timeProvider.Now, request.SignInCode))
        {
            return Error.Validation("Account.Login", "Account:InvalidCode");
        }

        var claimsPrincipal = await _userClaimsPrincipalFactory.CreateAsync(user);

        return GenerateToken(claimsPrincipal);
    }

    private LoginResult GenerateToken(ClaimsPrincipal claimsPrincipal)
    {
        var configuration = _configuration.GetSection("Authorization:JwtBearer");

        var key = Encoding.ASCII.GetBytes(configuration["Key"]!);
        var expires = _timeProvider.Now.AddMinutes(IdentityConsts.TokenExpirationInMinutes);

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = claimsPrincipal.Identity as ClaimsIdentity,
            Expires = expires,
            IssuedAt = _timeProvider.Now,
            Issuer = configuration["Issuer"],
            Audience = configuration["Audience"],
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var handler = new JwtSecurityTokenHandler();

        var token = handler.CreateToken(descriptor);

        return new LoginResult
        {
            AccessToken = handler.WriteToken(token),
            Expiration = expires.Ticks
        };
    }
}
