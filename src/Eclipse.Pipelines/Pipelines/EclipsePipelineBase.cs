using Eclipse.Application.Contracts.Localizations;
using Eclipse.Core.Pipelines;

using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines;

public abstract class EclipsePipelineBase : PipelineBase
{
    protected IEclipseLocalizer Localizer { get; private set; } = null!;

    protected IReadOnlyCollection<IReadOnlyCollection<KeyboardButton>> MainMenuButtons => new List<KeyboardButton[]>
    {
        new[] { new KeyboardButton(Localizer["Menu:MainMenu:MyToDos"]), new KeyboardButton(Localizer["Menu:MainMenu:Reminders"]) },
        new[] { new KeyboardButton(Localizer["Menu:MainMenu:Suggest"]), new KeyboardButton(Localizer["Menu:MainMenu:Settings"]) }
    };

    internal void SetLocalizer(IEclipseLocalizer localizer)
    {
        Localizer = localizer;
    }
}
