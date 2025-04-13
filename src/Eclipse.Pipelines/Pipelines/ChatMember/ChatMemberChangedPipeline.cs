using Eclipse.Application.Contracts.Users;
using Eclipse.Core.Context;
using Eclipse.Core.Results;

using Telegram.Bot.Types.Enums;

namespace Eclipse.Pipelines.Pipelines.ChatMember;

[BotEnabledPipeline]
[BotDisabledPipeline]
internal sealed class ChatMemberChangedPipeline : EclipsePipelineBase
{
    private readonly IUserService _userService;

    public ChatMemberChangedPipeline(IUserService userService)
    {
        _userService = userService;
    }

    protected override void Initialize()
    {
        RegisterStage(UpdateUserAsync);
    }

    private async Task<IResult> UpdateUserAsync(MessageContext context, CancellationToken cancellationToken)
    {
        var chatMember = Update.ChatMember;

        if (chatMember is null)
        {
            return Empty();
        }

        var user = await _userService.GetByChatIdAsync(context.ChatId, cancellationToken);

        if (!user.IsSuccess)
        {
            return Empty();
        }

        var model = new UserPartialUpdateDto
        {
            IsEnabled = chatMember.NewChatMember.Status == ChatMemberStatus.Member,
            IsEnabledChanged = true
        };

        await _userService.UpdatePartialAsync(user.Value.Id, model, cancellationToken);

        return Empty();
    }
}
