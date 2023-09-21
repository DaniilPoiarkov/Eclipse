namespace Eclipse.Application.Contracts;

public static class EclipseApplicationErrors
{
    public static class BotCommands
    {
        private static readonly string _prefix = "BotCommand";

        public static class Messages
        {
            public static readonly string CommandMinLength = $"{_prefix}:{nameof(CommandMinLength)}";

            public static readonly string CommandMaxLength = $"{_prefix}:{nameof(CommandMaxLength)}";

            public static readonly string DescriptionMinLength = $"{_prefix}:{nameof(DescriptionMinLength)}";

            public static readonly string DescriptionMaxLength = $"{_prefix}:{nameof(DescriptionMaxLength)}";
        }
    }

    public static class TodoItems
    {
        private static readonly string _prefix = "TodoItem";

        public static class Messages
        {
            public static readonly string Limit = "Limit";
        }
    }
}
