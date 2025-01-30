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
        Menu([buttons], message, inputPlaceholder, resize);

    protected static IResult Menu(IEnumerable<InlineKeyboardButton> buttons, string message) =>
        Menu([buttons], message);

    protected static IResult Menu(IReplyMarkup menu, string message) =>
        new MenuResult(message, menu);

    protected static IResult Menu(IEnumerable<IEnumerable<KeyboardButton>> buttons, string message, string inputPlaceholder = "", bool resize = true)
    {
        var hasButtons = buttons.SelectMany(x => x)
            .Any();

        if (!hasButtons)
        {
            return Text(message);
        }

        var menu = new ReplyKeyboardMarkup(buttons)
        {
            InputFieldPlaceholder = inputPlaceholder,
            ResizeKeyboard = resize
        };

        return new MenuResult(message, menu);
    }

    protected static IResult Menu(IEnumerable<IEnumerable<InlineKeyboardButton>> buttons, string message)
    {
        var hasButtons = buttons.SelectMany(x => x)
            .Any();

        if (!hasButtons)
        {
            return Text(message);
        }

        var menu = new InlineKeyboardMarkup(buttons);

        return new MenuResult(message, menu);
    }

    protected static IResult Redirect<TPipeline>(params IEnumerable<IResult> results)
        where TPipeline : Pipeline
    {
        return new RedirectResult(typeof(TPipeline), results);
    }

    protected static IResult Photo(MemoryStream stream, string fileName, string? caption = null)
    {
        return new PhotoResult(stream, fileName, caption);
    }
}
