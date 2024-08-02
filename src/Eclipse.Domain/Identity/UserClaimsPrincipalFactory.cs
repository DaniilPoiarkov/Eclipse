using Eclipse.Domain.Shared.Identity;
using Eclipse.Domain.Users;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

using System.Security.Claims;

namespace Eclipse.Domain.Identity;

internal sealed class UserClaimsPrincipalFactory : IUserClaimsPrincipalFactory<User>
{
    private readonly IConfiguration _configuration;

    public UserClaimsPrincipalFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<ClaimsPrincipal> CreateAsync(User user)
    {
        var role = user.ChatId == _configuration.GetValue<long>("Telegram:Chat")
            ? StaticRoleNames.Admin
            : StaticRoleNames.User;

        Claim[] claims = [
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Role, role)
        ];

        var identity = new ClaimsIdentity(claims);

        return Task.FromResult(new ClaimsPrincipal(identity));
    }
}
