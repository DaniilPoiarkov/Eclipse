using Eclipse.Domain.Shared.Identity;
using Eclipse.WebAPI.Authentication;
using Eclipse.WebAPI.Constants;
using Eclipse.WebAPI.Filters;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Eclipse.WebAPI.Configurations;

public class AuthorizationConfiguration : IConfigureOptions<AuthorizationOptions>
{
    public void Configure(AuthorizationOptions options)
    {
        var schemes = new[] { EclipseDefaults.AuthenticationScheme, ApiTokenAuthenticationHandler.SchemeName };

        options.AddPolicy(AuthorizationPolicies.Admin,
            policy => policy
                .AddAuthenticationSchemes(schemes)
                .RequireAssertion(context => context.User.IsInRole(StaticRoleNames.Admin))
        );

        options.DefaultPolicy = new AuthorizationPolicyBuilder(schemes)
            .RequireAuthenticatedUser()
            .Build();
    }
}
