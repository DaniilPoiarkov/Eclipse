using Eclipse.Core.Attributes;
using Eclipse.Core.Core;
using Eclipse.Infrastructure.Telegram;

namespace Eclipse.Pipelines.Pipelines.AdminMenu.SendMessage;

[Route("Send to user", "/send_to_user")]
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
        return Text("Send user chat Id");
    }

    private IResult AskForMessage(MessageContext context)
    {
        if (long.TryParse(context.Value, out var chatId))
        {
            MessageModel.ChatId = chatId;
            return Text("Send message content");
        }

        FinishPipeline();

        return Text("Unable too parse value. Pipeline interrupted");
    }

    private IResult Confirm(MessageContext context)
    {
        if (string.IsNullOrEmpty(context.Value))
        {
            FinishPipeline();
            return Menu(AdminMenuButtons, "Content cannot be empty. All data rolled back");
        }

        MessageModel.Message = context.Value;
        return Text("Send /confirm to send message or /cancel to go back");
    }

    private async Task<IResult> SendMessage(MessageContext context, CancellationToken cancellationToken)
    {
        if (!context.Value.Equals("/confirm", StringComparison.CurrentCultureIgnoreCase))
        {
            return Menu(AdminMenuButtons, "Message not sent. Confirmation failed");
        }

        try
        {
            await _telegramService.Send(MessageModel, cancellationToken);

            return Menu(AdminMenuButtons, "Sent successfully");
        }
        catch (Exception ex)
        {
            return Menu(AdminMenuButtons, $"Message not sent:{Environment.NewLine}" +
                $"{ex.Message}");
        }
    }
}
