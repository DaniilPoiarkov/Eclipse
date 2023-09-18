using Eclipse.Pipelines.Attributes;

using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines;

[AdminOnly]
public abstract class AdminPipelineBase : EclipsePipelineBase
{
    protected static readonly IReadOnlyCollection<KeyboardButton> AdminMenuButtons = new KeyboardButton[]
    {
        new KeyboardButton("View suggestions"),
        new KeyboardButton("View users"),
        new KeyboardButton("Switch to user mode"),
    };
}
