using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.MainMenu.Reports;

internal abstract class ReportsPipelineBase : EclipsePipelineBase
{
    protected IEnumerable<IEnumerable<KeyboardButton>> ReportsMenuButtons => new List<IList<KeyboardButton>>
    {
        new[] { new KeyboardButton(Localizer["Menu:Reports:Mood"]), new KeyboardButton(Localizer["Menu:Reports:Statistics"]) },
        new[] { new KeyboardButton(Localizer["Menu:MainMenu"]) }
    };
}
