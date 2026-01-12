using Eclipse.Common.Linq;
using Eclipse.Core.Pipelines;
using Eclipse.Core.Results;
using Eclipse.Domain.Promotions;

using Microsoft.Extensions.Localization;

using System.Collections.Generic;

using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines;

public abstract class EclipsePipelineBase : PipelineBase
{
    private IStringLocalizer? _localizer;
    protected IStringLocalizer Localizer => _localizer ?? throw new InvalidOperationException("String localizer was not provided.");

    protected IReadOnlyCollection<IReadOnlyCollection<KeyboardButton>> MainMenuButtons => new List<KeyboardButton[]>
    {
        new[] { new KeyboardButton(Localizer["Menu:MainMenu:Actions"]), new KeyboardButton(Localizer["Menu:MainMenu:Reports"]) },
        new[] { new KeyboardButton(Localizer["Menu:MainMenu:Suggest"]), new KeyboardButton(Localizer["Menu:MainMenu:Settings"]) }
    };

    internal void SetLocalizer(IStringLocalizer localizer)
    {
        _localizer = localizer;
    }

    protected static IResult RemoveMenuAndRedirect<TPipeline>(Message? message)
        where TPipeline : PipelineBase
    {
        if (message is null)
        {
            return Redirect<TPipeline>();
        }

        return Redirect<TPipeline>(
            Edit(message.MessageId, InlineKeyboardMarkup.Empty())
        );
    }

    protected IResult MenuAndClearPrevious(IEnumerable<IEnumerable<KeyboardButton>> buttons, Message? message, string text)
    {
        return MenuAndClearPrevious(
            new ReplyKeyboardMarkup(buttons)
            {
                ResizeKeyboard = true
            },
            message,
            text
        );
    }

    protected IResult MenuAndClearPrevious(ReplyMarkup buttons, Message? message, string text)
    {
        var menu = Menu(buttons, text);

        if (message is not { ReplyMarkup: { } })
        {
            return menu;
        }

        var edit = Edit(message.MessageId, InlineKeyboardMarkup.Empty());

        return Multiple(menu, edit);
    }

    protected List<IList<InlineKeyboardButton>> GetPagingButtons<T>(int page, PaginatedList<T> list)
    {
        List<IList<InlineKeyboardButton>> buttons = [];

        if (page > 1 && page < list.Pages)
        {
            buttons.Add([InlineKeyboardButton.WithCallbackData("◀️"), InlineKeyboardButton.WithCallbackData("▶️")]);
            buttons.Add([InlineKeyboardButton.WithCallbackData(Localizer["GoBack"], "go_back")]);
        }
        if (page == 1 && list.Pages > 1)
        {
            buttons.Add([InlineKeyboardButton.WithCallbackData("▶️")]);
            buttons.Add([InlineKeyboardButton.WithCallbackData(Localizer["GoBack"], "go_back")]);
        }
        if (page == list.Pages && list.Pages > 1)
        {
            buttons.Add([InlineKeyboardButton.WithCallbackData("◀️")]);
            buttons.Add([InlineKeyboardButton.WithCallbackData(Localizer["GoBack"], "go_back")]);
        }

        return buttons;
    }

    protected string GetPagingMessage(int page, int pagesCount, int totalCount, string message)
    {
        return Localizer["PagingInformation{Page}{PagesCount}{TotalCount}", page, pagesCount, totalCount]
            + Environment.NewLine
            + Environment.NewLine
            + message;
    }
}
