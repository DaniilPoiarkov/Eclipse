using Eclipse.Application.Contracts.TodoItems;
using Eclipse.Core.Core;

using System.Text;

using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.MainMenu.TodoItems;

internal abstract class TodoItemsPipelineBase : EclipsePipelineBase
{
    protected static readonly string PipelinePrefix = "Pipelines:TodoItems";

    protected IEnumerable<IEnumerable<KeyboardButton>> TodoItemMenuButtons => new List<KeyboardButton[]>
    {
        new[] { new KeyboardButton(Localizer["Menu:TodoItemsMenu:MyList"]), new KeyboardButton(Localizer["Menu:TodoItemsMenu:AddItem"]) },
        new[] { new KeyboardButton(Localizer["Menu:MainMenu"]) }
    };

    #region Helpers

    protected string BuildMessage(IEnumerable<TodoItemDto> items)
    {
        var sb = new StringBuilder(Localizer[$"{PipelinePrefix}:YourToDos"])
            .AppendLine()
            .AppendLine();

        foreach (var item in items)
        {
            sb.AppendLine($"📌 {item.Text}")
                .AppendLine($"{Localizer["CreatedAt"]}: {item.CreatedAt.ToString("dd.MM, HH:mm")}")
                .AppendLine();
        }

        return sb.AppendLine()
            .AppendLine(Localizer[$"{PipelinePrefix}:FinishOrGoBack"])
            .ToString();
    }

    protected List<InlineKeyboardButton[]> BuildButtons(IEnumerable<TodoItemDto> items)
    {
        var buttons = items
            .Select(item => InlineKeyboardButton.WithCallbackData(
                item.Text.Length < 25
                    ? item.Text
                    : $"{item.Text[..23]}..", $"{item.Id}"))
            .Select(button => new InlineKeyboardButton[] { button })
            .ToList();

        buttons.Add([InlineKeyboardButton.WithCallbackData(Localizer["GoBack"], "go_back")]);

        return buttons;
    }

    #endregion

    #region Results

    protected IResult AllItemsFinishedResult(Message? message)
    {
        var text = Localizer[$"{PipelinePrefix}:YouDidEmAll"];

        if (message is null)
        {
            return Text(text);
        }

        return Edit(message.MessageId, text);
    }

    protected IResult ItemFinishedResult(IEnumerable<TodoItemDto> leftover, Message message)
    {
        var buttons = BuildButtons(leftover);

        var text = $"{Localizer[$"{PipelinePrefix}:YouAreDoingGreat"]}" +
            $"{Environment.NewLine}{Environment.NewLine}" +
            $"{BuildMessage(leftover)}";

        var menu = new InlineKeyboardMarkup(buttons);

        return Edit(message.MessageId, text, menu);
    }

    protected IResult GoBackResult(Message? message) =>
        MenuOrMultipleResult(message, Localizer["WhateverYouWant"]);

    protected IResult InterruptedResult(Message? message, string text) =>
        MenuOrMultipleResult(message, text);

    private IResult MenuOrMultipleResult(Message? message, string text)
    {
        var menuResult = Menu(TodoItemMenuButtons, text);

        if (message is null)
        {
            return menuResult;
        }

        var editResult = Edit(message.MessageId, InlineKeyboardMarkup.Empty());

        return Multiple(menuResult, editResult);
    }

    #endregion
}
