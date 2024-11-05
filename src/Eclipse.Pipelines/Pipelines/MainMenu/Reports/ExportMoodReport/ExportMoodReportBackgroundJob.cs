using Eclipse.Application.Contracts.Reports;
using Eclipse.Application.Contracts.Users;
using Eclipse.Common.Background;
using Eclipse.Common.Clock;
using Eclipse.Localization.Culture;
using Eclipse.Localization.Extensions;

using Microsoft.Extensions.Localization;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Eclipse.Pipelines.Pipelines.MainMenu.Reports.ExportMoodReport;

internal sealed class ExportMoodReportBackgroundJob : IBackgroundJob<ExportMoodReportBackgroundJobArgs>
{
    private readonly IUserService _userService;

    private readonly IReportsService _reportsService;

    private readonly ITimeProvider _timeProvider;

    private readonly ITelegramBotClient _botClient;

    private readonly ICurrentCulture _currentCulture;

    private readonly IStringLocalizer<ExportMoodReportBackgroundJob> _localizer;

    public ExportMoodReportBackgroundJob(
        IUserService userService,
        IReportsService reportsService,
        ITimeProvider timeProvider,
        ITelegramBotClient botClient,
        ICurrentCulture currentCulture,
        IStringLocalizer<ExportMoodReportBackgroundJob> localizer)
    {
        _userService = userService;
        _reportsService = reportsService;
        _timeProvider = timeProvider;
        _botClient = botClient;
        _currentCulture = currentCulture;
        _localizer = localizer;
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
        _localizer.UseCurrentCulture(_currentCulture);

        var options = new MoodReportOptions
        {
            From = _timeProvider.Now.PreviousDayOfWeek(
                _timeProvider.Now.DayOfWeek
            ),
            To = _timeProvider.Now
        };

        using var stream = await _reportsService.GetMoodReportAsync(user.Id, options, cancellationToken);

        await _botClient.SendPhotoAsync(
            args.ChatId,
            InputFile.FromStream(stream, $"mood-report-{options.To.ToOADate()}.png"),
            caption: _localizer["Pipelines:Reports:Mood:Caption"],
            cancellationToken: cancellationToken
        );
    }
}
