using Eclipse.Application.Contracts.Telegram;
using Eclipse.Application.Contracts.Users;
using Eclipse.Common.Clock;
using Eclipse.Localization.Culture;

using Microsoft.Extensions.Localization;

using Quartz;

namespace Eclipse.Pipelines.Jobs.Evening;

internal sealed class RemindToFinishTodoItemsJob : EclipseJobBase
{
    private static readonly TimeOnly _evening = new(18, 0);

    private readonly IStringLocalizer<RemindToFinishTodoItemsJob> _localizer;

    private readonly IUserService _userService;

    private readonly ITelegramService _telegramService;

    private readonly ITimeProvider _timeProvider;

    private readonly ICurrentCulture _currentCulture;

    public RemindToFinishTodoItemsJob(
        IStringLocalizer<RemindToFinishTodoItemsJob> localizer,
        IUserService userService,
        ITelegramService telegramService,
        ITimeProvider timeProvider,
        ICurrentCulture currentCulture)
    {
        _localizer = localizer;
        _userService = userService;
        _telegramService = telegramService;
        _timeProvider = timeProvider;
        _currentCulture = currentCulture;
    }

    public override async Task Execute(IJobExecutionContext context)
    {
        var time = _timeProvider.Now.GetTime();

        var users = (await _userService.GetAllAsync(context.CancellationToken))
            .Where(u => u.NotificationsEnabled
                && time.Add(u.Gmt) == _evening)
            .ToList();

        if (users.IsNullOrEmpty())
        {
            return;
        }

        var sendings = new List<Task>(users.Count);

        foreach (var user in users)
        {
            using var _ = _currentCulture.UsingCulture(user.Culture);

            var template = _localizer[$"Jobs:Evening:{(user.TodoItems.IsNullOrEmpty() ? "Empty" : "RemindMarkAsFinished")}"];

            var message = user.TodoItems.IsNullOrEmpty()
                ? string.Format(template, user.Name)
                : string.Format(template, user.Name, user.TodoItems.Count);

            sendings.Add(_telegramService.Send(new SendMessageModel
            {
                ChatId = user.ChatId,
                Message = message,
            }, context.CancellationToken));
        }

        await Task.WhenAll(sendings);
    }
}
