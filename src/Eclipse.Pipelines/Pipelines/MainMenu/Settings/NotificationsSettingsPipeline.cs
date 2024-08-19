using Eclipse.Application.Contracts.Users;
using Eclipse.Application.Localizations;
using Eclipse.Core.Attributes;
using Eclipse.Core.Core;
using Eclipse.Pipelines.Stores.Messages;

using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.MainMenu.Settings;

[Route("Menu:Settings:Notifications", "/settings_notifications")]
internal sealed class NotificationsSettingsPipeline : SettingsPipelineBase
{
    private readonly IUserService _userService;

    private readonly IMessageStore _messageStore;

    private static readonly string _pipelinePrefix = "Pipelines:Settings:Notifications";

    public NotificationsSettingsPipeline(IUserService userService, IMessageStore messageStore)
    {
        _userService = userService;
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
        var message = await _messageStore.GetOrDefaultAsync(new MessageKey(context.ChatId), cancellationToken);

        var enable = context.Value.Equals("Enable");

        var result = await _userService.GetByChatIdAsync(context.ChatId, cancellationToken);

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

        var updateDto = new UserPartialUpdateDto
        {
            NotificationsEnabled = enable,
            NotificationsEnabledChanged = true,
        };

        var updateResult = await _userService.UpdatePartialAsync(user.Id, updateDto, cancellationToken);

        var text = updateResult.IsSuccess
            ? Localizer[$"{_pipelinePrefix}:{(enable ? "Enabled" : "Disabled")}"]
            : Localizer.LocalizeError(updateResult.Error);

        return MenuAndRemoveOptions(text, message?.MessageId);
    }
}
