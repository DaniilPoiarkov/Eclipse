using Eclipse.Core.Attributes;
using Eclipse.Core.Core;
using Eclipse.Pipelines.Stores.Messages;

using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.Daily;

[Route("", "/daily_morning")]
public sealed class MorningPipeline : EclipsePipelineBase
{
    private readonly IMessageStore _messageStore;

    public MorningPipeline(IMessageStore messageStore)
    {
        _messageStore = messageStore;
    }

    protected override void Initialize()
    {
        RegisterStage(AskMood);
        RegisterStage(HandleChoice);
    }

    private async Task<IResult> AskMood(MessageContext context, CancellationToken cancellationToken = default)
    {
        var buttons = new InlineKeyboardButton[]
        {
            InlineKeyboardButton.WithCallbackData("👍"),
            InlineKeyboardButton.WithCallbackData("👎"),
        };

        var message = await _messageStore.GetOrDefaultAsync(new MessageKey(context.ChatId), cancellationToken);

        return EditedOrDefaultResult(message, Menu(buttons, Localizer["Pipelines:Morning:AskMood"]));
    }

    private async Task<IResult> HandleChoice(MessageContext context, CancellationToken cancellationToken = default)
    {
        var text = context.Value switch
        {
            "👍" => "Pipelines:Morning:GoodMood",
            "👎" => "Pipelines:Morning:BadMood",
            _ => "Pipelines:Morning:NotDefined"
        };

        var message = await _messageStore.GetOrDefaultAsync(new MessageKey(context.ChatId), cancellationToken);

        return EditedOrDefaultResult(message, Text(Localizer[text]));
    }

    private static IResult EditedOrDefaultResult(Message? message, IResult @default)
    {
        return message is null || message.ReplyMarkup is null
            ? @default
            : Multiple(
                Edit(message.MessageId, InlineKeyboardMarkup.Empty()),
                @default
            );
    }
}
