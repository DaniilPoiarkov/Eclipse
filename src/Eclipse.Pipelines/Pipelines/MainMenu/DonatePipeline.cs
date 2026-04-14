using Eclipse.Core.Context;
using Eclipse.Core.Results;
using Eclipse.Core.Routing;

using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.MainMenu;

[Route("Menu:Donate", "/support")]
internal sealed class DonatePipeline : EclipsePipelineBase
{
    private readonly ITelegramBotClient _client;

    public DonatePipeline(ITelegramBotClient client)
    {
        _client = client;
    }

    protected override void Initialize()
    {
        RegisterStage(SendPaymentDetails);
    }

    private async Task<IResult> SendPaymentDetails(MessageContext context, CancellationToken cancellationToken)
    {
        await _client.SendMessage(
            chatId: context.ChatId,
            text: Localizer["Pipelines:Donations:Message"],
            replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl(
                text: Localizer["Pipelines:Donations:ButtonText"],
                url: Localizer["Pipelines:Donations:ButtonUrl"]
            )),
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken
        );

        return Empty();
    }
}
