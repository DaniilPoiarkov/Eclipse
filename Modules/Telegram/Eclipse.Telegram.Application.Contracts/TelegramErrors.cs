namespace Eclipse.Telegram.Application.Contracts;

public static class TelegramErrors
{
    private static readonly string _prefix = "Telegram";

    public static class Messages
    {
        public static readonly string MessageCannotBeEmpty = $"{_prefix}:MessageCannotBeEmpty";

        public static readonly string InvalidChatId = $"{_prefix}:InvalidChatId";
    }
}
