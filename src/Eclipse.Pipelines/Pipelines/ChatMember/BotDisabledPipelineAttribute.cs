using Eclipse.Core.Routing;

using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Eclipse.Pipelines.Pipelines.ChatMember;

internal sealed class BotDisabledPipelineAttribute : MyChatMemberPipelineAttribute
{
    protected override bool CanHandle(ChatMemberUpdated chatMember)
    {
        return chatMember.NewChatMember.Status == ChatMemberStatus.Kicked;
    }
}
