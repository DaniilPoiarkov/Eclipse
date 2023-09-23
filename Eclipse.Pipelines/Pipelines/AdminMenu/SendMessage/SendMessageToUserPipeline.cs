using Eclipse.Core.Attributes;
using Eclipse.Core.Core;
using Eclipse.Infrastructure.Telegram;

namespace Eclipse.Pipelines.Pipelines.AdminMenu.SendMessage;

[Route("Menu:AdminMenu:SendToUser", "/send_to_user")]
internal class SendMessageToUserPipeline : AdminPipelineBase
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

    private static IResult AskUserId(MessageContext context)
    {
        return Text(Localizer["Pipelines:AdminMenu:SendToUser:SendUserId"]);
    }

    private IResult AskForMessage(MessageContext context)
    {
        if (long.TryParse(context.Value, out var chatId))
        {
            MessageModel.ChatId = chatId;
            return Text(Localizer["Pipelines:AdminMenu:SendToUser:SendContent"]);
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
        if (!context.Value.Equals("/confirm", StringComparison.CurrentCultureIgnoreCase))
        {
            return Menu(AdminMenuButtons, Localizer["Pipelines:AdminMenu:ConfirmationFailed"]);
        }

        try
        {
            await _telegramService.Send(MessageModel, cancellationToken);

            return Menu(AdminMenuButtons, Localizer["Pipelines:AdminMenu:SentSuccessfully"]);
        }
        catch (Exception ex)
        {
            return Menu(AdminMenuButtons, $"{Localizer["Pipelines:AdminMenu:Error"]}:{Environment.NewLine}" +
                $"{ex.Message}");
        }
    }
}
