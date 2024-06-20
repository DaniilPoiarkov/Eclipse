using Eclipse.Application.Caching;
using Eclipse.Application.Contracts.Telegram;
using Eclipse.Application.Localizations;
using Eclipse.Common.Cache;
using Eclipse.Core.Attributes;
using Eclipse.Core.Core;

namespace Eclipse.Pipelines.Pipelines.AdminMenu.SendMessage;

[Route("Menu:AdminMenu:Send:User", "/admin_send_user")]
internal sealed class SendMessageToUserPipeline : AdminPipelineBase
{
    private readonly ITelegramService _telegramService;

    private readonly ICacheService _cacheService;

    public SendMessageToUserPipeline(ITelegramService telegramService, ICacheService cacheService)
    {
        _telegramService = telegramService;
        _cacheService = cacheService;
    }

    protected override void Initialize()
    {
        RegisterStage(AskUserId);
        RegisterStage(AskForMessage);
        RegisterStage(Confirm);
        RegisterStage(SendMessage);
    }

    private IResult AskUserId(MessageContext context)
    {
        return Text(Localizer["Pipelines:AdminMenu:SendToUser:SendUserId"]);
    }

    private async Task<IResult> AskForMessage(MessageContext context, CancellationToken cancellationToken)
    {
        if (long.TryParse(context.Value, out var chatId))
        {
            await _cacheService.SetAsync($"send-chat-{context.ChatId}", chatId, CacheConsts.ThreeDays, cancellationToken);
            return Text(Localizer["Pipelines:AdminMenu:SendContent"]);
        }

        FinishPipeline();
        return Text(Localizer["Pipelines:AdminMenu:SendToUser:UnableToParse"]);
    }

    private async Task<IResult> Confirm(MessageContext context, CancellationToken cancellationToken)
    {
        if (context.Value.IsNullOrEmpty())
        {
            FinishPipeline();
            return Menu(AdminMenuButtons, Localizer["Pipelines:AdminMenu:SendToUser:ContentCannotBeEmpty"]);
        }

        await _cacheService.SetAsync($"send-message-{context.ChatId}", context.Value, CacheConsts.ThreeDays, cancellationToken);

        return Text(Localizer["Pipelines:AdminMenu:Confirm"]);
    }

    private async Task<IResult> SendMessage(MessageContext context, CancellationToken cancellationToken)
    {
        var chatIdKey = $"send-chat-{context.ChatId}";
        var messageKey = $"send-message-{context.ChatId}";

        if (!context.Value.EqualsCurrentCultureIgnoreCase("/confirm"))
        {
            await _cacheService.DeleteAsync(chatIdKey, cancellationToken);
            await _cacheService.DeleteAsync(messageKey, cancellationToken);

            return Menu(AdminMenuButtons, Localizer["Pipelines:AdminMenu:ConfirmationFailed"]);
        }

        var chatId = await _cacheService.GetAndDeleteAsync<long>(chatIdKey, cancellationToken);
        var message = await _cacheService.GetAndDeleteAsync<string>(messageKey, cancellationToken);

        if (message.IsNullOrEmpty())
        {
            return Menu(AdminMenuButtons, Localizer["Pipelines:AdminMenu:SendToUser:ContentCannotBeEmpty"]);
        }

        var model = new SendMessageModel
        {
            ChatId = chatId,
            Message = message ?? string.Empty
        };

        var result = await _telegramService.Send(model, cancellationToken);

        if (result.IsSuccess)
        {
            return Menu(AdminMenuButtons, Localizer["Pipelines:AdminMenu:SentSuccessfully"]);
        }

        var text = $"{Localizer["Pipelines:AdminMenu:Error"]}:{Environment.NewLine}{Localizer.LocalizeError(result.Error)}";

        return Menu(AdminMenuButtons, text);
    }
}
