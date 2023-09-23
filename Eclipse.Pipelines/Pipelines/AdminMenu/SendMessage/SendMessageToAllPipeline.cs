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
        return Text(Localizer["Pipelines:AdminMenu:SendContent"]);
    }

    private IResult Confirm(MessageContext context)
    {
        if (string.IsNullOrEmpty(context.Value))
        {
            FinishPipeline();
            return Menu(AdminMenuButtons, Localizer["Pipelines:AdminMenu:SendToUser:ContentCannotBeEmpty"]);
        }

        Content = context.Value;
        return Text(Localizer["Pipelines:AdminMenu:Confirm"]);
    }

    private async Task<IResult> InformUsers(MessageContext context, CancellationToken cancellationToken)
    {
        if (!context.Value.Equals("/confirm", StringComparison.CurrentCultureIgnoreCase))
        {
            return Menu(AdminMenuButtons, Localizer["Pipelines:AdminMenu:ConfirmationFailed"]);
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

            return Menu(AdminMenuButtons, Localizer["Pipelines:AdminMenu:SentSuccessfully"]);
        }
        catch (Exception ex)
        {
            return Menu(AdminMenuButtons, $"{Localizer["Pipelines:AdminMenu:Error"]}:" +
                $"{Environment.NewLine}{ex.Message}");
        }
    }
}
