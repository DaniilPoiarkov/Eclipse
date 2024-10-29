using Eclipse.Core.Attributes;

namespace Eclipse.Pipelines.Pipelines.MainMenu.Actions.Reminders;

[Route("Menu:MainMenu:Reminders", "/reminders")]
internal sealed class RemindersPipeline : RemindersPipelineBase
{
    protected override void Initialize()
    {
        RegisterStage(_ => Menu(RemindersMenuButtons, Localizer["Pipelines:Reminders"]));
    }
}
