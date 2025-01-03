using Eclipse.Application.Contracts.Telegram;
using Eclipse.Application.Contracts.Users;
using Eclipse.Application.Localizations;
using Eclipse.Common.Caching;
using Eclipse.Core.Attributes;
using Eclipse.Core.Core;

namespace Eclipse.Pipelines.Pipelines.AdminMenu.SendMessage;

[Route("Menu:AdminMenu:Send:All", "/admin_send_all")]
internal sealed class SendMessageToAllPipeline : AdminPipelineBase
{
    private readonly IUserService _userService;

    private readonly ITelegramService _telegramService;

    private readonly ICacheService _cacheService;

    public SendMessageToAllPipeline(IUserService userService, ITelegramService telegramService, ICacheService cacheService)
    {
        _userService = userService;
        _telegramService = telegramService;
        _cacheService = cacheService;
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

    private async Task<IResult> Confirm(MessageContext context, CancellationToken cancellationToken)
    {
        if (context.Value.IsNullOrEmpty())
        {
            FinishPipeline();
            return Menu(AdminMenuButtons, Localizer["Pipelines:AdminMenu:SendToUser:ContentCannotBeEmpty"]);
        }

        await _cacheService.SetAsync($"send-all-{context.ChatId}", context.Value, CacheConsts.ThreeDays, [], cancellationToken);

        return Text(Localizer["Pipelines:AdminMenu:Confirm"]);
    }

    private async Task<IResult> InformUsers(MessageContext context, CancellationToken cancellationToken)
    {
        var messageKey = $"send-all-{context.ChatId}";

        if (!context.Value.EqualsCurrentCultureIgnoreCase("/confirm"))
        {
            await _cacheService.DeleteAsync(messageKey, cancellationToken);
            return Menu(AdminMenuButtons, Localizer["Pipelines:AdminMenu:ConfirmationFailed"]);
        }

        var message = await _cacheService.GetAndDeleteAsync<string>(messageKey, cancellationToken);

        if (message.IsNullOrEmpty())
        {
            return Menu(AdminMenuButtons, Localizer["Pipelines:AdminMenu:SendToUser:ContentCannotBeEmpty"]);
        }

        var notifications = (await _userService.GetAllAsync(cancellationToken))
            .Select(u => new SendMessageModel
            {
                ChatId = u.ChatId,
                Message = message
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
