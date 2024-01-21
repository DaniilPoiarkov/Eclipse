using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Core.Attributes;
using Eclipse.Core.Core;
using Eclipse.Pipelines.Stores.Messages;

using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.MainMenu.Settings;

[Route("Menu:Settings:Notifications", "/settings_notifications")]
internal class NotificationsSettingsPipeline : SettingsPipelineBase
{
    private readonly IIdentityUserService _identityUserService;

    private readonly IMessageStore _messageStore;

    public NotificationsSettingsPipeline(IIdentityUserService identityUserService, IMessageStore messageStore)
    {
        _identityUserService = identityUserService;
        _messageStore = messageStore;
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
        var message = _messageStore.GetOrDefault(new MessageKey(context.ChatId));

        var enable = context.Value.Equals("Enable");

        var user = await _identityUserService.GetByChatIdAsync(context.ChatId, cancellationToken);

        if (user.NotificationsEnabled.Equals(enable))
        {
            return MenuAndEditedOptionsMessage(
                Localizer[$"Pipelines:Settings:Notifications:Already{(enable ? "Enabled" : "Disabled")}"],
                message?.MessageId);
        }

        var updateDto = new IdentityUserUpdateDto
        {
            NotificationsEnabled = enable,
        };

        await _identityUserService.UpdateAsync(user.Id, updateDto, cancellationToken);

        return MenuAndEditedOptionsMessage(
            Localizer[$"Pipelines:Settings:Notifications:{(enable ? "Enabled" : "Disabled")}"],
            message?.MessageId);
    }
}
