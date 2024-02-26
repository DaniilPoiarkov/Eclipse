﻿using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Core.Attributes;
using Eclipse.Core.Core;
using Eclipse.Domain.Exceptions;
using Eclipse.Domain.IdentityUsers;

namespace Eclipse.Pipelines.Pipelines.MainMenu.Settings;

[Route("Menu:Settings:SetGmt", "/settings_setgmt")]
internal class SetGmtPipeline : SettingsPipelineBase
{
    private readonly IIdentityUserService _identityUserService;

    private static readonly string _pipelinePrefix = "Pipelines:Settings:SetGmt";

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
        var result = await _identityUserService.GetByChatIdAsync(context.ChatId, cancellationToken);

        if (!result.IsSuccess)
        {
            // TODO: Remove
            throw new EntityNotFoundException(typeof(IdentityUser));
        }

        var user = result.Value;

        var time = DateTime.UtcNow.GetTime();

        time = user.Gmt == default
            ? time
            : time.Add(user.Gmt);

        return Text(Localizer[$"{_pipelinePrefix}:Info"].Replace("{0}", $"{time:HH:mm}"));
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

        var userResult = await _identityUserService.GetByChatIdAsync(context.ChatId, cancellationToken);

        if (!userResult.IsSuccess)
        {
            // TODO: Remove
            throw new EntityNotFoundException(typeof(IdentityUser));
        }

        var result = await _identityUserService.SetUserGmtTimeAsync(userResult.Value.Id, time, cancellationToken);

        if (!result.IsSuccess)
        {
            // TODO: Remove. Make sense not even check this result
            throw new EntityNotFoundException(typeof(IdentityUser));
        }

        return Menu(SettingsMenuButtons, Localizer[$"{_pipelinePrefix}:Success"]);
    }
}
