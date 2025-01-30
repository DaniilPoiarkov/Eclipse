using Eclipse.Application.Contracts.Users;
using Eclipse.Common.Clock;
using Eclipse.Core.Attributes;
using Eclipse.Core.Core;

using System.Globalization;

namespace Eclipse.Pipelines.Pipelines.MainMenu.Actions.Reminders;

[Route("Menu:Reminders:List", "/reminders_list")]
internal sealed class RemindersListPipeline : RemindersPipelineBase
{
    private readonly IUserService _userService;

    private readonly ITimeProvider _timeProvider;

    public RemindersListPipeline(IUserService userService, ITimeProvider timeProvider)
    {
        _userService = userService;
        _timeProvider = timeProvider;
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

        var ordered = user.Reminders.OrderBy(r => r.NotifyAt);

        var current = TimeOnly.FromDateTime(_timeProvider.Now);
        var culture = CultureInfo.GetCultureInfo(user.Culture);

        var reminders = ordered.Where(r => r.NotifyAt >= current)
            .Concat(ordered.Where(r => r.NotifyAt < current))
            .Select(r => $"🕑 {r.NotifyAt.Add(user.Gmt).ToString(culture)} {r.Text}")
            .Join("\n\r");

        var message = Localizer["Pipelines:Reminders:List:Pending{0}", reminders];

        return Menu(RemindersMenuButtons, message);
    }
}
