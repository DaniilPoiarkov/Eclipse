using Eclipse.Application.Contracts.Users;
using Eclipse.Application.Localizations;
using Eclipse.Core.Attributes;
using Eclipse.Core.Core;

using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.MainMenu.Settings;

[Route("Menu:Settings:SetGmt", "/settings_setgmt")]
internal sealed class SetGmtPipeline : SettingsPipelineBase
{
    private readonly IUserService _userService;

    private static readonly string _pipelinePrefix = "Pipelines:Settings:SetGmt";

    public SetGmtPipeline(IUserService userService)
    {
        _userService = userService;
    }

    protected override void Initialize()
    {
        RegisterStage(SendKnownDataAndAskForNewGmt);
        RegisterStage(UpdateUserGmt);
    }

    private async Task<IResult> SendKnownDataAndAskForNewGmt(MessageContext context, CancellationToken cancellationToken)
    {
        var result = await _userService.GetByChatIdAsync(context.ChatId, cancellationToken);

        if (!result.IsSuccess)
        {
            return Text(Localizer.LocalizeError(result.Error));
        }

        var user = result.Value;

        var time = DateTime.UtcNow.GetTime();

        time = user.Gmt == default
            ? time
            : time.Add(user.Gmt);

        return Menu(new ReplyKeyboardRemove(), Localizer[$"{_pipelinePrefix}:Info", $"{time:HH:mm}"]);
    }

    private async Task<IResult> UpdateUserGmt(MessageContext context, CancellationToken cancellationToken)
    {
        if (context.Value.Equals("/cancel"))
        {
            return Menu(SettingsMenuButtons, Localizer["Okay"]);
        }

        if (!context.Value.TryParseAsTimeOnly(out var time))
        {
            return Menu(SettingsMenuButtons, Localizer[$"{_pipelinePrefix}:CannotParseTime"]);
        }

        var userResult = await _userService.GetByChatIdAsync(context.ChatId, cancellationToken);

        if (!userResult.IsSuccess)
        {
            return Menu(SettingsMenuButtons, Localizer.LocalizeError(userResult.Error));
        }

        var update = new UserPartialUpdateDto
        {
            Gmt = time,
            GmtChanged = true,
        };

        var result = await _userService.UpdatePartialAsync(userResult.Value.Id, update, cancellationToken);

        var text = result.IsSuccess
            ? Localizer[$"{_pipelinePrefix}:Success"]
            : Localizer.LocalizeError(result.Error);

        return Menu(SettingsMenuButtons, text);
    }
}
