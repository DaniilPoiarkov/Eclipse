using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Application.Contracts.Telegram;
using Eclipse.Application.Localizations;
using Eclipse.Core.Attributes;
using Eclipse.Core.Core;

namespace Eclipse.Pipelines.Pipelines.AdminMenu.SendMessage;

[Route("Menu:AdminMenu:Send:All", "/admin_send_all")]
internal sealed class SendMessageToAllPipeline : AdminPipelineBase
{
    private readonly IIdentityUserService _userService;

    private readonly ITelegramService _telegramService;

    private string Content { get; set; } = string.Empty;

    public SendMessageToAllPipeline(IIdentityUserService userService, ITelegramService telegramService)
    {
        _userService = userService;
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
        if (!context.Value.EqualsCurrentCultureIgnoreCase("/confirm"))
        {
            return Menu(AdminMenuButtons, Localizer["Pipelines:AdminMenu:ConfirmationFailed"]);
        }

        var notifications = (await _userService.GetAllAsync(cancellationToken))
            .Select(u => new SendMessageModel
            {
                ChatId = u.ChatId,
                Message = Content
            })
            .Select(m => _telegramService.Send(m, cancellationToken));

        var errors = (await Task.WhenAll(notifications))
            .Where(r => !r.IsSuccess)
            .Select(r => r.Error)
            .Select(Localizer.LocalizeError)
            .Select((error, index) => $"{index + 1}. {error}");

        if (errors.IsNullOrEmpty())
        {
            return Menu(AdminMenuButtons, Localizer["Pipelines:AdminMenu:SentSuccessfully"]);
        }

        var text = string.Join(Environment.NewLine, errors);

        return Menu(AdminMenuButtons, $"{Localizer["Pipelines:AdminMenu:Error"]}:{Environment.NewLine}{text}");
    }
}
