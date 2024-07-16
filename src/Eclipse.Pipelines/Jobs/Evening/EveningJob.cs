using Eclipse.Application.Contracts.Telegram;
using Eclipse.Localization.Culture;
using Eclipse.Pipelines.Users;
using Eclipse.Localization.Extensions;

using Microsoft.Extensions.Localization;

using Quartz;

namespace Eclipse.Pipelines.Jobs.Evening;

internal sealed class EveningJob : EclipseJobBase
{
    private static readonly TimeOnly Evening = new(18, 0);

    private readonly IStringLocalizer<EveningJob> _localizer;

    private readonly IUserStore _userStore;

    private readonly ITelegramService _telegramService;

    private readonly ICurrentCulture _currentCulture;

    public EveningJob(IStringLocalizer<EveningJob> localizer, IUserStore userStore, ITelegramService telegramService, ICurrentCulture currentCulture)
    {
        _localizer = localizer;
        _userStore = userStore;
        _telegramService = telegramService;
        _currentCulture = currentCulture;
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
            using var _ = _currentCulture.UsingCulture(user.Culture);
            _localizer.UseCurrentCulture(_currentCulture);

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
