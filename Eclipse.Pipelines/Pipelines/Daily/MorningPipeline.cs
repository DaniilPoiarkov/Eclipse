using Eclipse.Application.Contracts.Telegram.Messages;
using Eclipse.Core.Attributes;
using Eclipse.Core.Core;

using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.Daily;

[Route("", "/daily_morning")]
internal class MorningPipeline : EclipsePipelineBase
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

    private IResult AskMood(MessageContext context)
    {
        var text = Localizer["Pipelines:Morning:AskMood"];

        var buttons = new InlineKeyboardButton[]
        {
            InlineKeyboardButton.WithCallbackData("👍"),
            InlineKeyboardButton.WithCallbackData("👎"),
        };

        var message = _messageStore.GetMessage(new MessageKey(context.ChatId));

        var menu = Menu(buttons, text);

        if (message is null || message.ReplyMarkup is null)
        {
            return menu;
        }

        var edit = Edit(message.MessageId, InlineKeyboardMarkup.Empty());

        return Multiple(menu, edit);
    }

    private IResult HandleChoice(MessageContext context)
    {
        var message = context.Value switch
        {
            "👍" => "Pipelines:Morning:GoodMood",
            "👎" => "Pipelines:Morning:BadMood",
            _ => "Pipelines:Morning:NotDefined"
        };

        return Text(Localizer[message]);
    }
}
