using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Application.Extensions;
using Eclipse.Core.Attributes;
using Eclipse.Core.Core;

namespace Eclipse.Pipelines.Pipelines.MainMenu.Settings;

[Route("Menu:Settings:SetGmt", "/settings_setgmt")]
internal class SetGmtPipeline : SettingsPipelineBase
{
    private readonly IIdentityUserService _identityUserService;

    public SetGmtPipeline(IIdentityUserService identityUserService)
    {
        _identityUserService = identityUserService;
    }

    protected override void Initialize()
    {
        RegisterStage(SendKnownDataAndAskForNewGmt);
        RegisterStage(UpdateUserGmt);
    }

    private async Task<IResult> SendKnownDataAndAskForNewGmt(MessageContext context, CancellationToken cancellationToken)
    {
        var user = await _identityUserService.GetByChatIdAsync(context.ChatId, cancellationToken);

        var utc = DateTime.UtcNow;
        var time = new TimeOnly(utc.Hour, utc.Minute);

        time = user.Gmt == default
            ? time
            : time.Add(user.Gmt);

        return Text(Localizer["Pipelines:Settings:SetGmt:Info"].Replace("{0}", $"{time:HH:mm}"));
    }

    private async Task<IResult> UpdateUserGmt(MessageContext context, CancellationToken cancellationToken)
    {
        if (context.Value.Equals("/cancel"))
        {
            return Menu(SettingsMenuButtons, Localizer["Okay"]);
        }

        if (!context.Value.TryParseAsTimeOnly(out var time))
        {
            return Menu(SettingsMenuButtons, Localizer["Pipelines:Settings:SetGmt:CannotParseTime"]);
        }

        var user = await _identityUserService.GetByChatIdAsync(context.ChatId, cancellationToken);

        await _identityUserService.SetUserGmtTimeAsync(user.Id, time, cancellationToken);

        return Menu(SettingsMenuButtons, Localizer["Pipelines:Settings:SetGmt:Success"]);
    }
}
