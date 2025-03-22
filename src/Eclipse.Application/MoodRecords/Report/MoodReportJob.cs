using Eclipse.Application.Contracts.Reports;
using Eclipse.Common.Background;
using Eclipse.Common.Clock;
using Eclipse.Domain.Users;
using Eclipse.Localization.Culture;

using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace Eclipse.Application.MoodRecords.Report;

internal sealed class MoodReportJob : JobWithArgs<MoodReportJobData>
{
    private readonly IUserRepository _userRepository;

    private readonly ICurrentCulture _currentCulture;

    private readonly IReportsService _reportsService;

    private readonly ITelegramBotClient _client;

    private readonly ITimeProvider _timeProvider;

    private readonly IStringLocalizer<MoodReportJob> _localizer;

    public MoodReportJob(
        IUserRepository userRepository,
        ICurrentCulture currentCulture,
        IReportsService reportsService,
        ITelegramBotClient client,
        ITimeProvider timeProvider,
        IStringLocalizer<MoodReportJob> localizer,
        ILogger<MoodReportJob> logger) : base(logger)
    {
        _userRepository = userRepository;
        _currentCulture = currentCulture;
        _reportsService = reportsService;
        _client = client;
        _timeProvider = timeProvider;
        _localizer = localizer;
    }

    protected override async Task Execute(MoodReportJobData args, CancellationToken cancellationToken)
    {
        var user = await _userRepository.FindAsync(args.UserId, cancellationToken);

        if (user is null)
        {
            Logger.LogError("User with id {UserId} not found", args.UserId);
            return;
        }

        var options = new MoodReportOptions
        {
            From = _timeProvider.Now.PreviousDayOfWeek(DayOfWeek.Sunday),
            To = _timeProvider.Now,
        };

        using var _ = _currentCulture.UsingCulture(user.Culture);

        var message = _localizer["Jobs:SendMoodReport:Caption"];

        using var stream = await _reportsService.GetMoodReportAsync(args.UserId, options, cancellationToken);

        try
        {
            await _client.SendPhoto(user.ChatId,
                InputFile.FromStream(stream, $"mood-report.png"),
                caption: message,
                cancellationToken: cancellationToken
            );
        }
        catch (ApiRequestException ex)
        {
            Logger.LogError(ex, "Failed to send mood report for user {UserId}. Disabling user.", user.Id);

            user.SetIsEnabled(false);
            await _userRepository.UpdateAsync(user, cancellationToken);
        }
    }
}
