using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.MainMenu.TodoItems;

internal abstract class TodoItemsPipelineBase : EclipsePipelineBase
{
    protected static readonly IEnumerable<KeyboardButton> TodoItemMenuButtons = new KeyboardButton[]
    {
        new KeyboardButton("My list"),
        new KeyboardButton("Add item"),
        new KeyboardButton("Main menu"),
    };
}
