using Eclipse.Core.Pipelines;
using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines;

public abstract class EclipsePipelineBase : PipelineBase
{
    protected static readonly IReadOnlyCollection<KeyboardButton> MainMenuButtons = new KeyboardButton[]
    {
        new KeyboardButton("Suggest"),
        new KeyboardButton("My To dos"),
        new KeyboardButton("Test")
    };
}
