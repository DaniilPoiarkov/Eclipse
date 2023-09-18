﻿using Eclipse.Application.Contracts.Telegram.TelegramUsers;
using Eclipse.Core.Attributes;
using Eclipse.Core.Core;
using Eclipse.Pipelines.Attributes;

namespace Eclipse.Pipelines.Pipelines.AdminMenu;

[Route("", "/users")]
[AdminOnly]
public class UsersPipeline : EclipsePipelineBase
{
    private readonly ITelegramUserRepository _userRepository;

    public UsersPipeline(ITelegramUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    protected override void Initialize()
    {
        RegisterStage(GetUserInfo);
    }

    private IResult GetUserInfo(MessageContext context)
    {
        var usersInfo = _userRepository.GetAll()
            .Select((user, index) => $"{++index} | {user.Id} | {user.Name} | @{user.Username}");

        return Text(string.Join(Environment.NewLine, usersInfo));
    }
}
