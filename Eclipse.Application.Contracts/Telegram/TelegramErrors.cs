namespace Eclipse.Application.Contracts.Telegram;

public static class TelegramErrors
{
    private static readonly string _prefix = "Telegram";

    public static class Messages
    {
        public static readonly string MessageCannotBeEmpty = $"{_prefix}:MessageCannotBeEmpty";
    }
}
