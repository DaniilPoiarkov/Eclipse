﻿using Eclipse.Application.Contracts.TodoItems;
using Eclipse.Core.Core;

using System.Text;

using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.MainMenu.TodoItems;

internal abstract class TodoItemsPipelineBase : EclipsePipelineBase
{
    protected static readonly IEnumerable<KeyboardButton> TodoItemMenuButtons = new KeyboardButton[]
    {
        new KeyboardButton(Localizer["Menu:TodoItemsMenu:MyList"]),
        new KeyboardButton(Localizer["Menu:TodoItemsMenu:AddItem"]),
        new KeyboardButton(Localizer["Menu:MainMenu"]),
    };

    #region Helpers

    protected static string BuildMessage(IEnumerable<TodoItemDto> items)
    {
        var sb = new StringBuilder(Localizer["Pipelines:TodoItems:YourToDos"])
            .AppendLine()
            .AppendLine();

        foreach (var item in items)
        {
            sb.AppendLine($"⬜️ {item.Text}")
                .AppendLine($"{Localizer["Pipelines:TodoItems:CreatedAt"]} {item.CreatedAt.ToString("dd.MM, HH:mm")}")
                .AppendLine();
        }

        return sb.AppendLine()
            .AppendLine(Localizer["Pipelines:TodoItems:FinishOrGoBack"])
            .ToString();
    }

    protected static IEnumerable<IEnumerable<InlineKeyboardButton>> BuildButtons(IEnumerable<TodoItemDto> items)
    {
        var buttons = items
            .Select(item => InlineKeyboardButton.WithCallbackData(
                item.Text.Length < 25
                    ? item.Text
                    : $"{item.Text[..23]}..", $"{item.Id}"))
            .Select(button => new InlineKeyboardButton[] { button })
            .ToList();

        buttons.Add(new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData(Localizer["GoBack"], "go_back") });

        return buttons;
    }

    #endregion

    #region Results

    protected static IResult AllItemsFinishedResult(Message? message)
    {
        var text = Localizer["Pipelines:TodoItems:YouDidEmAll"];

        if (message is null)
        {
            return Text(text);
        }

        return Edit(message.MessageId, text);
    }

    protected static IResult ItemFinishedResult(IEnumerable<TodoItemDto> leftover, Message message)
    {
        var buttons = BuildButtons(leftover);

        var text = $"{Localizer["Pipelines:TodoItems:YouAreDoingGreat"]}" +
            $"{Environment.NewLine}{Environment.NewLine}" +
            $"{BuildMessage(leftover)}";

        var menu = new InlineKeyboardMarkup(buttons);

        return Edit(message.MessageId, text, menu);
    }

    protected static IResult GoBackResult(Message? message) =>
        MenuOrMultipleResult(message, Localizer["WhateverYouWant"]);

    protected static IResult InterruptedResult(Message? message, string text) =>
        MenuOrMultipleResult(message, text);

    private static IResult MenuOrMultipleResult(Message? message, string text)
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
