using Eclipse.Pipelines.Attributes;

using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines;

[AdminOnly]
public abstract class AdminPipelineBase : EclipsePipelineBase
{
    protected static readonly IReadOnlyCollection<IReadOnlyCollection<KeyboardButton>> AdminMenuButtons = new List<KeyboardButton[]>
    {
        new[] { new KeyboardButton("View suggestions"), new KeyboardButton("View users") },
        new[] { new KeyboardButton("Send to user"), new KeyboardButton("Send to all") },
        new[] { new KeyboardButton("Switch to user mode") },
    };
}
