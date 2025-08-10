using Eclipse.Application.Contracts.TodoItems;
using Eclipse.Core.Results;

using System.Globalization;
using System.Text;

using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.MainMenu.Actions.TodoItems;

internal abstract class TodoItemsPipelineBase : ActionsPipelineBase
{
    protected static readonly string PipelinePrefix = "Pipelines:TodoItems";

    protected IEnumerable<IEnumerable<KeyboardButton>> TodoItemMenuButtons => new List<KeyboardButton[]>
    {
        new[] { new KeyboardButton(Localizer["Menu:TodoItems:List"]), new KeyboardButton(Localizer["Menu:TodoItems:AddItem"]) },
        new[] { new KeyboardButton(Localizer["Menu:MainMenu:Actions"]) }
    };

    protected string BuildMessage(TimeSpan userTime, CultureInfo culture, IEnumerable<TodoItemDto> items)
    {
        var sb = new StringBuilder(Localizer[$"{PipelinePrefix}:YourToDos"])
            .AppendLine()
            .AppendLine();

        foreach (var item in items)
        {
            sb.AppendLine($"📌 {item.Text}")
                .AppendLine($"{Localizer["CreatedAt"]}: {item.CreatedAt.Add(userTime).ToString(culture)}")
                .AppendLine();
        }

        return sb.AppendLine()
            .AppendLine(Localizer[$"{PipelinePrefix}:FinishOrGoBack"])
            .ToString();
    }

    protected InlineKeyboardButton[][] BuildButtons(IEnumerable<TodoItemDto> items)
    {
        var buttons = items
            .Select(item => InlineKeyboardButton.WithCallbackData(
                item.Text.Length < 25
                    ? item.Text
                    : $"{item.Text[..23]}..", $"{item.Id}"))
            .Select(button => new InlineKeyboardButton[] { button })
            .Append([InlineKeyboardButton.WithCallbackData(Localizer["GoBack"], "go_back")])
            .ToArray();

        return buttons;
    }

    protected IResult AllItemsFinishedResult(Message? message)
    {
        var text = Localizer[$"{PipelinePrefix}:YouDidEmAll"];

        if (message is null)
        {
            return Text(text);
        }

        return Edit(message.MessageId, text);
    }

    protected IResult ItemFinishedResult(TimeSpan userTime, CultureInfo culture, IEnumerable<TodoItemDto> leftover, Message message)
    {
        var buttons = BuildButtons(leftover);

        var text = $"{Localizer[$"{PipelinePrefix}:YouAreDoingGreat"]}" +
            $"{Environment.NewLine}{Environment.NewLine}" +
            $"{BuildMessage(userTime, culture, leftover)}";

        var menu = new InlineKeyboardMarkup(buttons);

        return Edit(message.MessageId, text, menu);
    }

    protected IResult GoBackResult(Message? message) =>
        MenuAndClearPrevious(TodoItemMenuButtons, message, Localizer["WhateverYouWant"]);

    protected IResult InterruptedResult(Message? message, string text) =>
        MenuAndClearPrevious(TodoItemMenuButtons, message, text);
}
