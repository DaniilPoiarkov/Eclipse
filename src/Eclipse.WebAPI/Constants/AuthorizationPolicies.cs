namespace Eclipse.WebAPI.Constants;

public static class AuthorizationPolicies
{
    public const string Admin = "admin";

    public static class Scopes
    {
        public const string Reminders = "scope:reminders";
        public const string TodoItems = "scope:todo-items";
        public const string UserStatistics = "scope:user-statistics";
        public const string MoodRecords = "scope:mood-records";
        public const string Cache = "scope:cache";
        public const string Commands = "scope:commands";
        public const string Export = "scope:export";
        public const string Import = "scope:import";
        public const string Suggestions = "scope:suggestions";
        public const string Telegram = "scope:telegram";
        public const string Users = "scope:users";
        public const string InboxMessages = "scope:inbox-messages";
        public const string Feedbacks = "scope:feedbacks";
        public const string Promotions = "scope:promotions";
    }
}
