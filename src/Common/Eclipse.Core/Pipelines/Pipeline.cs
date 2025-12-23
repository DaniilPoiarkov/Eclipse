using Eclipse.Core.Results;

using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Core.Pipelines;

public abstract class Pipeline
{
    internal protected Update Update
    {
        get => field ?? throw new InvalidOperationException($"{nameof(Update)} is null.");
        set
        {
            ArgumentNullException.ThrowIfNull(value, nameof(value));
            field = value;
        }
    }

    public void SetUpdate(Update update) => Update = update;

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

    protected static IResult Menu(ReplyMarkup menu, string message) =>
        new MenuResult(message, menu);
    protected static IResult Menu(IEnumerable<IEnumerable<KeyboardButton>> buttons, string message, string inputPlaceholder = "", bool resize = true)
    {
        var hasButtons = buttons.SelectMany(x => x).Any();

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

    protected static IResult Redirect<TPipeline>(params IEnumerable<IResult> results)
        where TPipeline : Pipeline
    {
        return new RedirectResult(typeof(TPipeline), results);
    }

    protected static IResult Photo(MemoryStream stream, string fileName, string? caption = null) =>
        new PhotoResult(stream, fileName, caption);

    protected static IResult RemoveInlineMenuAndSend(ReplyMarkup menu, string text, Message? message)
    {
        if (message?.ReplyMarkup is null || !message.ReplyMarkup.InlineKeyboard.Any())
        {
            return Menu(menu, text);
        }

        return Multiple(
            Edit(message.MessageId, InlineKeyboardMarkup.Empty()),
            Menu(menu, text)
        );
    }

    protected static IResult RemoveInlineMenuAndSend(string text, Message? message)
    {
        if (message?.ReplyMarkup is null || !message.ReplyMarkup.InlineKeyboard.Any())
        {
            return Text(text);
        }

        return Multiple(
            Edit(message.MessageId, InlineKeyboardMarkup.Empty()),
            Text(text)
        );
    }
}
