using Eclipse.Domain.Shared.ApiTokens;
using Eclipse.Domain.Shared.Identity;
using Eclipse.WebAPI.Authentication;
using Eclipse.WebAPI.Authorization;
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

        options.AddPolicy(AuthorizationPolicies.Scopes.ApiTokens,
            policy => policy.AddRequirements(new ApiTokenScopeRequirement(ApiTokenScope.ApiTokens)));

        options.AddPolicy(AuthorizationPolicies.Scopes.Reminders,
            policy => policy.AddRequirements(new ApiTokenScopeRequirement(ApiTokenScope.Reminders)));

        options.AddPolicy(AuthorizationPolicies.Scopes.TodoItems,
            policy => policy.AddRequirements(new ApiTokenScopeRequirement(ApiTokenScope.TodoItems)));

        options.AddPolicy(AuthorizationPolicies.Scopes.UserStatistics,
            policy => policy.AddRequirements(new ApiTokenScopeRequirement(ApiTokenScope.UserStatistics)));

        options.AddPolicy(AuthorizationPolicies.Scopes.MoodRecords,
            policy => policy.AddRequirements(new ApiTokenScopeRequirement(ApiTokenScope.MoodRecords)));

        options.AddPolicy(AuthorizationPolicies.Scopes.Cache,
            policy => policy.AddRequirements(new ApiTokenScopeRequirement(ApiTokenScope.Cache)));

        options.AddPolicy(AuthorizationPolicies.Scopes.Commands,
            policy => policy.AddRequirements(new ApiTokenScopeRequirement(ApiTokenScope.Commands)));

        options.AddPolicy(AuthorizationPolicies.Scopes.Export,
            policy => policy.AddRequirements(new ApiTokenScopeRequirement(ApiTokenScope.Export)));

        options.AddPolicy(AuthorizationPolicies.Scopes.Import,
            policy => policy.AddRequirements(new ApiTokenScopeRequirement(ApiTokenScope.Import)));

        options.AddPolicy(AuthorizationPolicies.Scopes.Suggestions,
            policy => policy.AddRequirements(new ApiTokenScopeRequirement(ApiTokenScope.Suggestions)));

        options.AddPolicy(AuthorizationPolicies.Scopes.Telegram,
            policy => policy.AddRequirements(new ApiTokenScopeRequirement(ApiTokenScope.Telegram)));

        options.AddPolicy(AuthorizationPolicies.Scopes.Users,
            policy => policy.AddRequirements(new ApiTokenScopeRequirement(ApiTokenScope.Users)));

        options.AddPolicy(AuthorizationPolicies.Scopes.InboxMessages,
            policy => policy.AddRequirements(new ApiTokenScopeRequirement(ApiTokenScope.InboxMessages)));

        options.AddPolicy(AuthorizationPolicies.Scopes.Feedbacks,
            policy => policy.AddRequirements(new ApiTokenScopeRequirement(ApiTokenScope.Feedbacks)));

        options.AddPolicy(AuthorizationPolicies.Scopes.Promotions,
            policy => policy.AddRequirements(new ApiTokenScopeRequirement(ApiTokenScope.Promotions)));
    }
}
