using System.ComponentModel;
using System.Reflection;

using Eclipse.Domain.Shared.Identity;

namespace Eclipse.Domain.Shared.ApiTokens;

public static class ApiTokenScopeHelper
{
    public static string GetDescription(ApiTokenScope scope)
    {
        var member = typeof(ApiTokenScope).GetField(scope.ToString());
        return member?.GetCustomAttribute<DescriptionAttribute>()?.Description ?? string.Empty;
    }

    public static IReadOnlyList<ApiTokenScopeInfo> GetAvailableScopeInfos(string role) =>
        [.. GetAvailableScopes(role).Select(s => new ApiTokenScopeInfo(s.ToString(), GetDescription(s)))];

    public static readonly IReadOnlyList<ApiTokenScope> UserScopes =
    [
        ApiTokenScope.Reminders,
        ApiTokenScope.TodoItems,
        ApiTokenScope.UserStatistics,
        ApiTokenScope.MoodRecords,
    ];

    public static readonly IReadOnlyList<ApiTokenScope> AdminScopes =
    [
        ApiTokenScope.Cache,
        ApiTokenScope.Commands,
        ApiTokenScope.Export,
        ApiTokenScope.Import,
        ApiTokenScope.Suggestions,
        ApiTokenScope.Telegram,
        ApiTokenScope.Users,
        ApiTokenScope.InboxMessages,
        ApiTokenScope.Feedbacks,
        ApiTokenScope.Promotions,
    ];

    public static IReadOnlyList<ApiTokenScope> GetAvailableScopes(string role)
    {
        if (role == StaticRoleNames.User)
        {
            return UserScopes;
        }
        if (role == StaticRoleNames.Admin)
        {
            return [..UserScopes, ..AdminScopes];
        }

        return [];
    }
}
