using System.ComponentModel;

namespace Eclipse.Domain.Shared.ApiTokens;

public enum ApiTokenScope
{
    // Never included in UserScopes or AdminScopes — no token should carry this scope
    [Description("Manage API tokens: create, list, and revoke tokens")]
    ApiTokens,

    // User-level
    [Description("Manage reminders: create, list, update, and delete reminders")]
    Reminders,

    [Description("Manage todo items: create, list, update, and delete tasks")]
    TodoItems,

    [Description("Access user statistics and activity summaries")]
    UserStatistics,

    [Description("Manage mood records: create, list, update, and delete mood entries")]
    MoodRecords,

    // Admin-level
    [Description("Manage application cache: clear and inspect cached data")]
    Cache,

    [Description("Manage bot commands: register and update Telegram bot command list")]
    Commands,

    [Description("Export user and application data")]
    Export,

    [Description("Import user and application data")]
    Import,

    [Description("Manage suggestions submitted by users")]
    Suggestions,

    [Description("Manage Telegram integration settings and operations")]
    Telegram,

    [Description("Manage users: list, update, and query user accounts")]
    Users,

    [Description("Manage inbox messages")]
    InboxMessages,

    [Description("View and manage user feedback submissions")]
    Feedbacks,

    [Description("Manage promotional messages and campaigns")]
    Promotions,
}
