using Eclipse.Core.Attributes;

using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.MainMenu;

[Route("My To dos", "/todos")]
public class TodoItemsMenuPipeline : EclipsePipelineBase
{
    private static readonly IEnumerable<KeyboardButton> _buttons = new KeyboardButton[]
    {
        new KeyboardButton("My list"),
        new KeyboardButton("Add item"),
        new KeyboardButton("Main menu"),
    };

    private static readonly string _message = "Let's take a look what do we have here..";

    protected override void Initialize()
    {
        RegisterStage(_ => Menu(_buttons, _message));
    }
}
