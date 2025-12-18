using Eclipse.Application.Contracts.Reminders;
using Eclipse.Application.Reminders.Sendings;
using Eclipse.Localization.Culture;

using Microsoft.Extensions.Localization;

using Telegram.Bot;

namespace Eclipse.Pipelines.Reminders;

internal sealed class SingleTimeReminderStrategy : IReminderSenderStrategy
{
    private readonly IReminderService _reminderService;

    private readonly ITelegramBotClient _client;

    private readonly ICurrentCulture _currentCulture;

    private readonly IStringLocalizer<ReminderSender> _localizer;

    public SingleTimeReminderStrategy(
        IReminderService reminderService,
        ITelegramBotClient client,
        ICurrentCulture currentCulture,
        IStringLocalizer<ReminderSender> localizer)
    {
        _reminderService = reminderService;
        _client = client;
        _currentCulture = currentCulture;
        _localizer = localizer;
    }

    public async Task Send(ReminderArguments arguments, CancellationToken cancellationToken = default)
    {
        using var _ = _currentCulture.UsingCulture(arguments.Culture);

        await _client.SendMessage(
            arguments.ChatId,
            $"{_localizer["Jobs:SendReminders:Message"]}\n\r\n\r{arguments.Text}",
            cancellationToken: cancellationToken
        );

        await _reminderService.DeleteAsync(arguments.UserId, arguments.ReminderId, cancellationToken);
    }
}
