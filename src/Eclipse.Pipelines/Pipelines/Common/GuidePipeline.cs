using Eclipse.Core.Context;
using Eclipse.Core.Results;
using Eclipse.Core.Routing;

using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.Common;

[Route("Menu:Guide", "/guide")]
internal sealed class GuidePipeline : EclipsePipelineBase
{
    private static readonly TimeSpan _seconds = TimeSpan.FromSeconds(1.5);

    private readonly ITelegramBotClient _telegramBotClient;

    public GuidePipeline(ITelegramBotClient telegramBotClient)
    {
        _telegramBotClient = telegramBotClient;
    }

    protected override void Initialize()
    {
        RegisterStage(SendGuide);
    }

    private async Task<IResult> SendGuide(MessageContext messageContext, CancellationToken cancellationToken)
    {
        foreach (var key in new[] { "Guide:1", "Guide:2", "Guide:3", "Guide:4" })
        {
            await SendSegment(messageContext.ChatId, key, cancellationToken);
        }

        await _telegramBotClient.SendMessage(
            messageContext.ChatId,
            Localizer["Guide:Menu"],
            replyMarkup: new ReplyKeyboardMarkup(MainMenuButtons) { ResizeKeyboard = true },
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken
        );

        return Empty();
    }

    private async Task SendSegment(long chatId, string key, CancellationToken cancellationToken)
    {
        await _telegramBotClient.SendMessage(chatId, Localizer[key], parseMode: ParseMode.Html, cancellationToken: cancellationToken);
        await _telegramBotClient.SendChatAction(chatId, ChatAction.Typing, cancellationToken: cancellationToken);
        await Task.Delay(_seconds, cancellationToken);
    }
}
