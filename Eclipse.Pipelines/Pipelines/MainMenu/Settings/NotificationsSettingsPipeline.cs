using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Core.Attributes;
using Eclipse.Core.Core;
using Eclipse.Domain.Shared.IdentityUsers;

using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.MainMenu.Settings;

[Route("Menu:Settings:Notifications", "/settings_notifications")]
internal class NotificationsSettingsPipeline : SettingsPipelineBase
{
    private readonly IIdentityUserService _identityUserService;

    public NotificationsSettingsPipeline(IIdentityUserService identityUserService)
    {
        _identityUserService = identityUserService;
    }

    protected override void Initialize()
    {
        RegisterStage(SendOptions);
        RegisterStage(EnableNotifications);
    }

    private IResult SendOptions(MessageContext context)
    {
        var buttons = new InlineKeyboardButton[]
        {
            InlineKeyboardButton.WithCallbackData($"{Localizer["Enable"]} 🔉", "Enable"),
            InlineKeyboardButton.WithCallbackData($"{Localizer["Disable"]} 🔇")
        };

        return Menu(buttons, Localizer["Pipelines:Settings:Notifications:Message"]);
    }

    private async Task<IResult> EnableNotifications(MessageContext context, CancellationToken cancellationToken)
    {
        var enable = context.Value.Equals("Enable");

        var user = await _identityUserService.GetByChatIdAsync(context.ChatId, cancellationToken);

        if (user.NotificationsEnabled && enable)
        {
            return Text(Localizer["Pipelines:Settings:Notifications:AlreadyEnabled"]);
        }

        if (!user.NotificationsEnabled && !enable)
        {
            return Text(Localizer["Pipelines:Settings:Notifications:AlreadyDisabled"]);
        }

        var updateDto = new IdentityUserUpdateDto
        {
            NotificationsEnabled = enable,
        };

        await _identityUserService.UpdateAsync(user.Id, updateDto, cancellationToken);

        return Text(Localizer[$"Pipelines:Settings:Notifications:{(enable ? "Enabled" : "Disabled")}"]);
    }
}
