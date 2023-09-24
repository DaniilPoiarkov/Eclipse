namespace Eclipse.Application.Telegram;

internal static class TelegramErrors
{
    private static readonly string _prefix = "Telegram";

    public static class Messages
    {
        public static readonly string BotAlreadyRunning = $"{_prefix}:BotAlreadyRunning";

        public static readonly string MessageCannotBeEmpty = $"{_prefix}:MessageCannotBeEmpty";
    }
}
