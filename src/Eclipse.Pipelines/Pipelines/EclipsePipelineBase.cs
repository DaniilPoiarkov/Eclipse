using Eclipse.Core.Pipelines;
using Eclipse.Core.Results;

using Microsoft.Extensions.Localization;

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

    protected IResult MenuOrMultipleResult(IEnumerable<IEnumerable<KeyboardButton>> buttons, Message? message, string text)
    {
        var menuResult = Menu(buttons, text);

        if (message is null)
        {
            return menuResult;
        }

        var editResult = Edit(message.MessageId, InlineKeyboardMarkup.Empty());

        return Multiple(menuResult, editResult);
    }
}
