using Eclipse.Core.Attributes;
using Eclipse.Core.Core;
using Eclipse.Infrastructure.Telegram;

namespace Eclipse.Pipelines.Pipelines.AdminMenu.SendMessage;

[Route("Send to user", "/send_to_user")]
internal class SendMessageToUserPipeline : AdminPipelineBase
{
    private readonly ITelegramService _telegramService;

    private long ChatId;

    public SendMessageToUserPipeline(ITelegramService telegramService)
    {
        _telegramService = telegramService;
    }

    protected override void Initialize()
    {
        RegisterStage(AskUserId);
        RegisterStage(AskForMessage);
        RegisterStage(SendMessage);
    }

    private static IResult AskUserId(MessageContext context)
    {
        return Text("Send user chat Id");
    }

    private IResult AskForMessage(MessageContext context)
    {
        if (long.TryParse(context.Value, out ChatId))
        {
            return Text("Send message content");
        }

        FinishPipeline();

        return Text("Unable too parse value. Pipeline interrupted");
    }

    private async Task<IResult> SendMessage(MessageContext context, CancellationToken cancellationToken)
    {
        var model = new SendMessageModel
        {
            ChatId = ChatId,
            Message = context.Value
        };

        try
        {
            await _telegramService.Send(model, cancellationToken);

            return Menu(AdminMenuButtons, "Sent successfully");
        }
        catch (Exception ex)
        {
            return Menu(AdminMenuButtons, $"Message not sent:{Environment.NewLine}" +
                $"{ex.Message}");
        }
    }
}
