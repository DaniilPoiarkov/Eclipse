using Eclipse.Core.Attributes;
using Eclipse.Pipelines.Pipelines.MainMenu.Reminders;

namespace Eclipse.Pipelines.Pipelines.MainMenu;

[Route("Menu:MainMenu:Reminders", "/reminders")]
internal class RemindersPipeline : RemindersPipelineBase
{
    protected override void Initialize()
    {
        RegisterStage(_ => Menu(RemindersMenuButtons, Localizer["Pipelines:Reminders"]));
    }
}
