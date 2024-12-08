using Eclipse.Application.Contracts.Users;
using Eclipse.Core.Attributes;
using Eclipse.Core.Core;

namespace Eclipse.Pipelines.Pipelines.MainMenu.Actions.Reminders;

[Route("Menu:Reminders:List", "/reminders_list")]
internal sealed class RemindersListPipeline : RemindersPipelineBase
{
    private readonly IUserService _userService;

    public RemindersListPipeline(IUserService userService)
    {
        _userService = userService;
    }

    protected override void Initialize()
    {
        RegisterStage(SendPendingReminders);
    }

    private async Task<IResult> SendPendingReminders(MessageContext context, CancellationToken cancellationToken)
    {
        var result = await _userService.GetByChatIdAsync(context.ChatId, cancellationToken);

        if (!result.IsSuccess)
        {
            return Menu(RemindersMenuButtons, Localizer["Error"]);
        }

        var user = result.Value;

        if (user.Reminders.IsNullOrEmpty())
        {
            return Menu(RemindersMenuButtons, Localizer["Pipelines:Reminders:List:Empty"]);
        }

        var message = Localizer[
            "Pipelines:Reminders:List:Pending{0}",
            user.Reminders
                .Select(r => $"🕑 {r.NotifyAt.Add(user.Gmt)} {r.Text}")
                .Join("\n\r")
        ];

        return Menu(RemindersMenuButtons, message);
    }
}
