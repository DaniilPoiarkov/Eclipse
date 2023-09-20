using Eclipse.Application.Contracts.Telegram.TelegramUsers;
using Eclipse.Core.Attributes;
using Eclipse.Core.Core;
using Eclipse.Infrastructure.Telegram;

namespace Eclipse.Pipelines.Pipelines.AdminMenu.SendMessage;

[Route("Send to all", "/send_to_all")]
internal class SendMessageToAllPipeline : AdminPipelineBase
{
    private readonly ITelegramUserRepository _telegramUserRepository;

    private readonly ITelegramService _telegramService;

    public SendMessageToAllPipeline(ITelegramUserRepository telegramUserRepository, ITelegramService telegramService)
    {
        _telegramUserRepository = telegramUserRepository;
        _telegramService = telegramService;
    }

    protected override void Initialize()
    {
        RegisterStage(AskForMessage);
        RegisterStage(InformUsers);
    }

    private IResult AskForMessage(MessageContext context)
    {
        return Text("Send message content");
    }

    private async Task<IResult> InformUsers(MessageContext context, CancellationToken cancellationToken)
    {
        try
        {
            var notifications = _telegramUserRepository.GetAll()
            .Select(u => new SendMessageModel
            {
                ChatId = u.Id,
                Message = context.Value
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
