using System.Diagnostics.CodeAnalysis;

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
            { InlineQuery.From: { } } => update.InlineQuery.From,
            { ChosenInlineResult.From: { } } => update.ChosenInlineResult.From,
            { MyChatMember.From: { } } => update.MyChatMember.From,
            { ChatMember.From: { } } => update.ChatMember.From,
            _ => throw new InvalidOperationException("Unsupported update type for extracting sender.")
        };
    }

    internal static bool TryExtractMessage(this Update update, [NotNullWhen(true)] out Message? message)
    {
        message = update switch
        {
            { Message: { } tgMessage } => tgMessage,
            { CallbackQuery.Message: { } tgMessage } => tgMessage,
            _ => null
        };

        return message is not null;
    }
}
