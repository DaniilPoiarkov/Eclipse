using Eclipse.Core.Pipelines;

using Microsoft.Extensions.Localization;

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
}
