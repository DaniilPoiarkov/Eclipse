using Eclipse.Application.Contracts.Telegram.TelegramUsers;
using Eclipse.Core.Attributes;
using Eclipse.Core.Core;
using Eclipse.Infrastructure.Telegram;

namespace Eclipse.Pipelines.Pipelines.AdminMenu.SendMessage;

[Route("Menu:AdminMenu:SendToAll", "/send_to_all")]
internal class SendMessageToAllPipeline : AdminPipelineBase
{
    private readonly ITelegramUserRepository _telegramUserRepository;

    private readonly ITelegramService _telegramService;

    private string Content { get; set; } = string.Empty;

    public SendMessageToAllPipeline(ITelegramUserRepository telegramUserRepository, ITelegramService telegramService)
    {
        _telegramUserRepository = telegramUserRepository;
        _telegramService = telegramService;
    }

    protected override void Initialize()
    {
        RegisterStage(AskForMessage);
        RegisterStage(Confirm);
        RegisterStage(InformUsers);
    }

    private IResult AskForMessage(MessageContext context)
    {
        return Text("Send message content");
    }

    private IResult Confirm(MessageContext context)
    {
        if (string.IsNullOrEmpty(context.Value))
        {
            FinishPipeline();
            return Menu(AdminMenuButtons, "Content cannot be empty. All data rolled back");
        }

        Content = context.Value;
        return Text("Send /confirm to send message or /cancel to go back");
    }

    private async Task<IResult> InformUsers(MessageContext context, CancellationToken cancellationToken)
    {
        if (!context.Value.Equals("/confirm", StringComparison.CurrentCultureIgnoreCase))
        {
            return Menu(AdminMenuButtons, "Message not sent. Confirmation failed");
        }

        try
        {
            var notifications = _telegramUserRepository.GetAll()
            .Select(u => new SendMessageModel
            {
                ChatId = u.Id,
                Message = Content
            })
            .Select(m => _telegramService.Send(m, cancellationToken));

            await Task.WhenAll(notifications);

            return Menu(AdminMenuButtons, "Sent successfully");
        }
        catch (Exception ex)
        {
            return Menu(AdminMenuButtons, $"Message not sent:" +
                $"{Environment.NewLine}{ex.Message}");
        }
    }
}
