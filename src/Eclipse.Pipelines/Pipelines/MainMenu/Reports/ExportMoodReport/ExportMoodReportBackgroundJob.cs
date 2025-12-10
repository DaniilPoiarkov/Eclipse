using Eclipse.Application.Contracts.MoodRecords;
using Eclipse.Application.Contracts.Users;
using Eclipse.Common.Background;
using Eclipse.Common.Clock;
using Eclipse.Localization.Culture;

using Microsoft.Extensions.Localization;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Eclipse.Pipelines.Pipelines.MainMenu.Reports.ExportMoodReport;

internal sealed class ExportMoodReportBackgroundJob : IBackgroundJob<ExportMoodReportBackgroundJobArgs>
{
    private readonly IUserService _userService;

    private readonly IMoodReportService _reportsService;

    private readonly ITimeProvider _timeProvider;

    private readonly ITelegramBotClient _botClient;

    private readonly IStringLocalizer<ExportMoodReportBackgroundJob> _localizer;

    private readonly ICurrentCulture _currentCulture;

    public ExportMoodReportBackgroundJob(
        IUserService userService,
        IMoodReportService reportsService,
        ITimeProvider timeProvider,
        ITelegramBotClient botClient,
        IStringLocalizer<ExportMoodReportBackgroundJob> localizer,
        ICurrentCulture currentCulture)
    {
        _userService = userService;
        _reportsService = reportsService;
        _timeProvider = timeProvider;
        _botClient = botClient;
        _localizer = localizer;
        _currentCulture = currentCulture;
    }

    public async Task ExecuteAsync(ExportMoodReportBackgroundJobArgs args, CancellationToken cancellationToken = default)
    {
        var result = await _userService.GetByChatIdAsync(args.ChatId, cancellationToken);

        if (!result.IsSuccess)
        {
            return;
        }

        var user = result.Value;

        using var _ = _currentCulture.UsingCulture(user.Culture);

        var options = new MoodReportOptions
        {
            From = _timeProvider.Now.PreviousDayOfWeek(
                _timeProvider.Now.DayOfWeek
            ).WithTime(0, 0),
            To = _timeProvider.Now.WithTime(23, 59)
        };

        using var stream = await _reportsService.GetAsync(user.Id, options, cancellationToken);

        await _botClient.SendPhoto(
            args.ChatId,
            InputFile.FromStream(stream, $"mood-report-{options.To.ToOADate()}.png"),
            caption: _localizer["Pipelines:Reports:Mood:Caption"],
            cancellationToken: cancellationToken
        );
    }
}
