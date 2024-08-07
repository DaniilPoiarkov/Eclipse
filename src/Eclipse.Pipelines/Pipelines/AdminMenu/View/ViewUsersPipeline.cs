﻿using Eclipse.Application.Contracts.Users;
using Eclipse.Core.Attributes;
using Eclipse.Core.Core;

namespace Eclipse.Pipelines.Pipelines.AdminMenu.View;

[Route("Menu:AdminMenu:View:Users", "/admin_view_users")]
internal sealed class ViewUsersPipeline : AdminPipelineBase
{
    private readonly IUserService _userService;

    public ViewUsersPipeline(IUserService userService)
    {
        _userService = userService;
    }

    protected override void Initialize()
    {
        RegisterStage(GetUserInfo);
    }

    private async Task<IResult> GetUserInfo(MessageContext context, CancellationToken cancellationToken = default)
    {
        var usersInfo = (await _userService.GetAllAsync(cancellationToken))
            .Select((user, index) => $"{++index} | {user.ChatId} | {user.Name} {user.UserName.FormattedOrEmpty(s => $"| @{s}")}");

        return Text(string.Join(Environment.NewLine, usersInfo));
    }
}
