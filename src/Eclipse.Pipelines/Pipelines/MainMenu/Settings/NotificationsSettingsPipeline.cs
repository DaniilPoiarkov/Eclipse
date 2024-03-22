using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Application.Localizations;
using Eclipse.Core.Attributes;
using Eclipse.Core.Core;
using Eclipse.Pipelines.Stores.Messages;

using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.MainMenu.Settings;

[Route("Menu:Settings:Notifications", "/settings_notifications")]
internal sealed class NotificationsSettingsPipeline : SettingsPipelineBase
{
    private readonly IIdentityUserService _identityUserService;

    private readonly IMessageStore _messageStore;

    private static readonly string _pipelinePrefix = "Pipelines:Settings:Notifications";

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

        return Menu(buttons, Localizer[$"{_pipelinePrefix}:Message"]);
    }

    private async Task<IResult> EnableNotifications(MessageContext context, CancellationToken cancellationToken)
    {
        var message = _messageStore.GetOrDefault(new MessageKey(context.ChatId));

        var enable = context.Value.Equals("Enable");

        var result = await _identityUserService.GetByChatIdAsync(context.ChatId, cancellationToken);

        if (!result.IsSuccess)
        {
            return MenuAndRemoveOptions(
                Localizer.LocalizeError(result.Error),
                message?.MessageId);
        }

        var user = result.Value;

        if (user.NotificationsEnabled.Equals(enable))
        {
            return MenuAndRemoveOptions(
                Localizer[$"{_pipelinePrefix}:Already{(enable ? "Enabled" : "Disabled")}"],
                message?.MessageId);
        }

        var updateDto = new IdentityUserUpdateDto
        {
            NotificationsEnabled = enable,
        };

        var updateResult = await _identityUserService.UpdateAsync(user.Id, updateDto, cancellationToken);

        var text = updateResult.IsSuccess
            ? Localizer[$"{_pipelinePrefix}:{(enable ? "Enabled" : "Disabled")}"]
            : Localizer.LocalizeError(updateResult.Error);

        return MenuAndRemoveOptions(text, message?.MessageId);
    }
}
