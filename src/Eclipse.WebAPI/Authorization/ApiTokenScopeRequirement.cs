using Eclipse.Domain.Shared.ApiTokens;

using Microsoft.AspNetCore.Authorization;

namespace Eclipse.WebAPI.Authorization;

public sealed class ApiTokenScopeRequirement : IAuthorizationRequirement
{
    public ApiTokenScope Scope { get; }

    public ApiTokenScopeRequirement(ApiTokenScope scope)
    {
        Scope = scope;
    }
}
