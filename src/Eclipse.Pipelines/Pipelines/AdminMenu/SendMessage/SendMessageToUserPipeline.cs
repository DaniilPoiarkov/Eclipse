using Eclipse.Application.Contracts.Telegram;
using Eclipse.Application.Localizations;
using Eclipse.Core.Attributes;
using Eclipse.Core.Core;

namespace Eclipse.Pipelines.Pipelines.AdminMenu.SendMessage;

[Route("Menu:AdminMenu:Send:User", "/admin_send_user")]
internal sealed class SendMessageToUserPipeline : AdminPipelineBase
{
    private readonly ITelegramService _telegramService;

    private SendMessageModel MessageModel { get; set; } = new();

    public SendMessageToUserPipeline(ITelegramService telegramService)
    {
        _telegramService = telegramService;
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

    private IResult AskForMessage(MessageContext context)
    {
        if (long.TryParse(context.Value, out var chatId))
        {
            MessageModel.ChatId = chatId;
            return Text(Localizer["Pipelines:AdminMenu:SendContent"]);
        }

        FinishPipeline();
        return Text(Localizer["Pipelines:AdminMenu:SendToUser:UnableToParse"]);
    }

    private IResult Confirm(MessageContext context)
    {
        if (string.IsNullOrEmpty(context.Value))
        {
            FinishPipeline();
            return Menu(AdminMenuButtons, Localizer["Pipelines:AdminMenu:SendToUser:ContentCannotBeEmpty"]);
        }

        MessageModel.Message = context.Value;
        return Text(Localizer["Pipelines:AdminMenu:Confirm"]);
    }

    private async Task<IResult> SendMessage(MessageContext context, CancellationToken cancellationToken)
    {
        if (!context.Value.EqualsCurrentCultureIgnoreCase("/confirm"))
        {
            return Menu(AdminMenuButtons, Localizer["Pipelines:AdminMenu:ConfirmationFailed"]);
        }

        var result = await _telegramService.Send(MessageModel, cancellationToken);

        if (result.IsSuccess)
        {
            return Menu(AdminMenuButtons, Localizer["Pipelines:AdminMenu:SentSuccessfully"]);
        }

        var text = $"{Localizer["Pipelines:AdminMenu:Error"]}:{Environment.NewLine}{Localizer.LocalizeError(result.Error)}";

        return Menu(AdminMenuButtons, text);
    }
}
