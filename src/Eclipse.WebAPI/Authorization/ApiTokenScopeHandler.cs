using Eclipse.Domain.Shared.ApiTokens;
using Eclipse.WebAPI.Authentication;

using Microsoft.AspNetCore.Authorization;

namespace Eclipse.WebAPI.Authorization;

public sealed class ApiTokenScopeHandler : AuthorizationHandler<ApiTokenScopeRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ApiTokenScopeRequirement requirement)
    {
        var identity = context.User.Identity;

        if (identity is null || !identity.IsAuthenticated)
        {
            return Task.CompletedTask;
        }

        if (identity.AuthenticationType != ApiTokenAuthenticationHandler.SchemeName)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        if (context.User.HasClaim(ApiTokenClaimTypes.Scope, requirement.Scope.ToString()))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
