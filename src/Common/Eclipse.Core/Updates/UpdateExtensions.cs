using Telegram.Bot.Types;

namespace Eclipse.Core.Updates;

public static class UpdateExtensions
{
    /// <summary>
    /// Extracts the sender.
    /// </summary>
    /// <param name="update">The update.</param>
    /// <returns></returns>
    public static User ExtractSender(this Update update)
    {
        return update switch
        {
            { Message.From: { } } => update.Message.From,
            { CallbackQuery.From: { } } => update.CallbackQuery.From,
            { InlineQuery.From: { } } => update.InlineQuery.From,
            { ChosenInlineResult.From: { } } => update.ChosenInlineResult.From,
            { MyChatMember.From: { } } => update.MyChatMember.From,
            { ChatMember.From: { } } => update.ChatMember.From,
            _ => throw new InvalidOperationException("Unsupported update type for extracting sender.")
        };
    }
}
