using Eclipse.Core.Attributes;
using Eclipse.Core.Core;
using Eclipse.Core.Results;
using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Core.Pipelines;

[Pipeline]
public abstract class Pipeline
{
    protected static IResult Empty() => new EmptyResult();

    protected static IResult Text(string text) => new TextResult(text);

    protected static IResult Menu(IEnumerable<KeyboardButton> buttons, string message, string inputPlaceholder = "", bool resize = true) =>
            Menu(new List<IEnumerable<KeyboardButton>>() { buttons }, message, inputPlaceholder, resize);

    protected static IResult Menu(IEnumerable<IEnumerable<KeyboardButton>> buttons, string message, string inputPlaceholder = "", bool resize = true)
    {
        var menu = new ReplyKeyboardMarkup(buttons)
        {
            InputFieldPlaceholder = inputPlaceholder,
            ResizeKeyboard = resize
        };

        return new MenuResult(message, menu);
    }
}
