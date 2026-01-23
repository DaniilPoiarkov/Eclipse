using Telegram.Bot.Types;

namespace Eclipse.Pipelines.UpdateHandler;

internal static class UpdateExtensions
{
    internal static User ExtractSender(this Update update)
    {
        return update switch
        {
            { Message.From: { } } => update.Message.From,
            { CallbackQuery.From: { } } => update.CallbackQuery.From,
            { MyChatMember.From: { } } => update.MyChatMember.From,
            { ChatMember.From: { } } => update.ChatMember.From,
            _ => throw new InvalidOperationException("Unsupported update type for extracting sender.")
        };
    }
}
