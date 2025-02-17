using Eclipse.Domain.Shared.Identity;
using Eclipse.WebAPI.Constants;
using Eclipse.WebAPI.Filters;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Eclipse.WebAPI.Configurations;

public class AuthorizationConfiguration : IConfigureOptions<AuthorizationOptions>
{
    public void Configure(AuthorizationOptions options)
    {
        options.AddPolicy(AuthorizationPolicies.Admin,
            policy => policy.RequireAssertion(
                context => context.User.IsInRole(StaticRoleNames.Admin)
            )
        );

        options.DefaultPolicy = new AuthorizationPolicyBuilder(EclipseDefaults.AuthenticationScheme)//, JwtBearerDefaults.AuthenticationScheme
            .RequireAuthenticatedUser()
            .Build();
    }
}
