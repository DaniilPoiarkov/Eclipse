using Eclipse.Core.Context;
using Eclipse.Core.Results;
using Eclipse.Core.Routing;

using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.MainMenu.ApiTokens;

[Route("Menu:ApiTokens", "/api_tokens")]
internal sealed class ApiTokensPipeline : ApiTokensPipelineBase
{
    private readonly ITelegramBotClient _client;

    public ApiTokensPipeline(ITelegramBotClient client)
    {
        _client = client;
    }

    protected override void Initialize()
    {
        RegisterStage(SendApiTokensInfo);
    }

    private async Task<IResult> SendApiTokensInfo(MessageContext context, CancellationToken cancellationToken)
    {
        await _client.SendMessage(
            chatId: context.ChatId,
            text: Localizer["Pipelines:ApiTokens"],
            replyMarkup: new ReplyKeyboardMarkup(ApiTokensMenuButtons) { ResizeKeyboard = true },
            parseMode: ParseMode.Markdown,
            cancellationToken: cancellationToken
        );

        return Empty();
    }
}
