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


    protected static IResult Edit(int messageId, string text) =>
        new EditTextResult(messageId, text);

    protected static IResult Edit(int messageId, InlineKeyboardMarkup menu) =>
        new EditMenuResult(messageId, menu);

    protected static IResult Edit(int messageId, string text, InlineKeyboardMarkup menu) =>
        new MultipleActionsResult(
        [
            Edit(messageId, text),
            Edit(messageId, menu)
        ]);

    protected static IResult Multiple(params IResult[] results) =>
        new MultipleActionsResult(results);

    protected static IResult Menu(IEnumerable<KeyboardButton> buttons, string message, string inputPlaceholder = "", bool resize = true) =>
        Menu(new List<IEnumerable<KeyboardButton>>() { buttons }, message, inputPlaceholder, resize);

    protected static IResult Menu(IEnumerable<InlineKeyboardButton> buttons, string message) =>
        Menu(new List<IEnumerable<InlineKeyboardButton>>() { buttons }, message);

    protected static IResult Menu(IEnumerable<IEnumerable<KeyboardButton>> buttons, string message, string inputPlaceholder = "", bool resize = true)
    {
        var button = buttons.FirstOrDefault()?
            .FirstOrDefault();

        if (button is null)
        {
            return Text(message);
        }

        var keyboardButtons = buttons.Cast<IEnumerable<KeyboardButton>>();

        var menu = new ReplyKeyboardMarkup(keyboardButtons)
        {
            InputFieldPlaceholder = inputPlaceholder,
            ResizeKeyboard = resize
        };

        return new MenuResult(message, menu);
    }

    protected static IResult Menu(
        IEnumerable<IEnumerable<InlineKeyboardButton>> buttons,
        string message)
    {
        var button = buttons.FirstOrDefault()?
            .FirstOrDefault();

        if (button is null)
        {
            return Text(message);
        }

        var inlineMenu = new InlineKeyboardMarkup(buttons);

        return new MenuResult(message, inlineMenu);
    }
}
