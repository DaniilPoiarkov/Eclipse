using Eclipse.Core.Attributes;
using Eclipse.Core.Core;

using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.Daily;

[Route("", "/daily_morning")]
internal class MorningPipeline : EclipsePipelineBase
{
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

        return Menu(buttons, text);
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
