using Eclipse.Core.Context;
using Eclipse.Core.Results;
using Eclipse.Core.Routing;

using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.Common;

[Route("", "/mcp")]
internal sealed class McpPipeline : EclipsePipelineBase
{
    private readonly ITelegramBotClient _client;

    public McpPipeline(ITelegramBotClient client)
    {
        _client = client;
    }

    protected override void Initialize()
    {
        RegisterStage(SendMcpInstructions);
    }

    private async Task<IResult> SendMcpInstructions(MessageContext context, CancellationToken cancellationToken)
    {
        await _client.SendMessage(
            chatId: context.ChatId,
            text: Localizer["Pipelines:Common:Mcp:Message"],
            replyMarkup: new InlineKeyboardMarkup(
                InlineKeyboardButton.WithUrl(Localizer["Pipelines:Common:Mcp:ButtonText"], Localizer["Pipelines:Common:Mcp:ButtonUrl"])
            ),
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken
        );

        return Empty();
    }
}
