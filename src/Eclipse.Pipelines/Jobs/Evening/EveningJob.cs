using Eclipse.Application.Contracts.Telegram;
using Eclipse.Localization.Localizers;
using Eclipse.Pipelines.Users;

using Quartz;

namespace Eclipse.Pipelines.Jobs.Evening;

internal sealed class EveningJob : EclipseJobBase
{
    private static readonly TimeOnly Evening = new(18, 0);

    private readonly ILocalizer _localizer;

    private readonly IUserStore _userStore;

    private readonly ITelegramService _telegramService;

    public EveningJob(ILocalizer localizer, IUserStore userStore, ITelegramService telegramService)
    {
        _localizer = localizer;
        _userStore = userStore;
        _telegramService = telegramService;
    }

    public override async Task Execute(IJobExecutionContext context)
    {
        var time = DateTime.UtcNow.GetTime();

        var users = (await _userStore.GetCachedUsersAsync(context.CancellationToken))
            .Where(u => u.NotificationsEnabled
                && time.Add(u.Gmt) == Evening)
            .ToList();

        if (users.IsNullOrEmpty())
        {
            return;
        }

        var sendings = new List<Task>(users.Count);

        foreach (var user in users)
        {
            var template = _localizer[$"Jobs:Evening:{(user.TodoItems.IsNullOrEmpty() ? "Empty" : "RemindMarkAsFinished")}", user.Culture];

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
