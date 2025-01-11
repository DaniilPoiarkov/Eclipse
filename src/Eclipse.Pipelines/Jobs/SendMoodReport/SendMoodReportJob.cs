using Eclipse.Application.Reports;
using Eclipse.Common.Clock;
using Eclipse.Common.Plots;
using Eclipse.Domain.MoodRecords;
using Eclipse.Domain.Shared.MoodRecords;
using Eclipse.Domain.Users;
using Eclipse.Localization.Culture;

using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

using Quartz;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Eclipse.Pipelines.Jobs.SendMoodReport;

internal sealed class SendMoodReportJob : EclipseJobBase
{
    private static readonly TimeOnly _time = new(19, 30);

    private static readonly TimeOnly _recordTime = new(0, 0);

    private static readonly int _width = 550;

    private static readonly int _height = 300;

    private static readonly int _oneMinute = 1;


    private readonly IUserRepository _userRepository;

    private readonly IMoodRecordRepository _moodRecordRepository;

    private readonly ITelegramBotClient _client;

    private readonly IStringLocalizer<SendMoodReportJob> _localizer;

    private readonly ITimeProvider _timeProvider;

    private readonly IPlotGenerator _plotGenerator;

    private readonly ICurrentCulture _currentCulture;

    private readonly ILogger<SendMoodReportJob> _logger;

    public SendMoodReportJob(
        IUserRepository userRepository,
        IMoodRecordRepository moodRecordRepository,
        ITelegramBotClient client,
        IStringLocalizer<SendMoodReportJob> localizer,
        ITimeProvider timeProvider,
        IPlotGenerator plotGenerator,
        ICurrentCulture currentCulture,
        ILogger<SendMoodReportJob> logger)
    {
        _userRepository = userRepository;
        _moodRecordRepository = moodRecordRepository;
        _client = client;
        _localizer = localizer;
        _timeProvider = timeProvider;
        _plotGenerator = plotGenerator;
        _currentCulture = currentCulture;
        _logger = logger;
    }

    public override async Task Execute(IJobExecutionContext context)
    {
        try
        {

            var date = _timeProvider.Now;

            if (date.DayOfWeek != DayOfWeek.Sunday)
            {
                _logger.LogInformation("Rescheduling job {Job} to the next Sunday.", nameof(SendMoodReportJob));
                await RescheduleJobAsync(context, date);
                return;
            }

            var time = date.GetTime();

            var users = (await _userRepository.GetByExpressionAsync(u => u.NotificationsEnabled, context.CancellationToken))
                .Where(u => time.Add(u.Gmt) == _time)
                .ToList();

            var after = date.PreviousDayOfWeek(DayOfWeek.Sunday);

            var moodRecords = await _moodRecordRepository.GetByExpressionAsync(
                mr => users.Select(u => u.Id).Contains(mr.UserId) && mr.CreatedAt >= after,
                context.CancellationToken
            );

            var pairs = moodRecords
                .OrderBy(mr => mr.CreatedAt)
                .GroupBy(mr => mr.UserId)
                .Join(users, group => group.Key, user => user.Id, (group, user) => (User: user, Options: GeneratePlotOptions([.. group], user.Gmt)));

            var sending = new List<Task>(users.Count);

            foreach (var pair in pairs)
            {
                var user = pair.User;
                var options = pair.Options;

                using var _ = _currentCulture.UsingCulture(user.Culture);

                var message = _localizer["Jobs:SendMoodReport:Caption"].Value;

                sending.Add(
                    SendReportAsync(user.ChatId, options, message, context.CancellationToken)
                );
            }

            await Task.WhenAll(sending);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send weekly mood report job. {Exception}", ex);
        }
    }

    private PlotOptions<DateTime, int> GeneratePlotOptions(List<MoodRecord> records, TimeSpan userGmt)
    {
        var days = new DateTime[records.Count];
        var states = new int[records.Count];

        for (int i = 0; i < records.Count; i++)
        {
            var record = records[i];

            days[i] = record.CreatedAt.WithTime(_recordTime);
            states[i] = record.State.ToScore();
        }

        var (from, to) = days.IsNullOrEmpty()
            ? (_timeProvider.Now.Add(userGmt), _timeProvider.Now.Add(userGmt))
            : (days[0], days[^1]);

        return new PlotOptions<DateTime, int>
        {
            Title = $"{from:dd.MM}-{to:dd.MM}",
            Left = new AxisOptions<int>
            {
                Label = "Score",
                Values = states,
                Max = MoodState.Good.ToScore(),
                Min = MoodState.Bad.ToScore(),
            },
            Bottom = new AxisOptions<DateTime>
            {
                Values = days,
            },
            Width = _width,
            Height = _height,
        };
    }

    private async Task SendReportAsync(long chatId, PlotOptions<DateTime, int> options, string message, CancellationToken cancellationToken = default)
    {
        using var stream = _plotGenerator.Create(options);

        await _client.SendPhoto(chatId,
            InputFile.FromStream(stream, $"mood-report.png"),
            caption: message,
            cancellationToken: cancellationToken
        );
    }

    private static async Task RescheduleJobAsync(IJobExecutionContext context, DateTime date)
    {
        var scheduler = context.Scheduler;

        var trigger = TriggerBuilder.Create()
            .ForJob(context.JobDetail.Key)
            .StartAt(date.NextDayOfWeek(DayOfWeek.Sunday).WithTime(0, 0))
            .WithSimpleSchedule(schedule => schedule
                .WithIntervalInMinutes(_oneMinute)
                .RepeatForever())
            .Build();

        await scheduler.RescheduleJob(context.Trigger.Key, trigger, context.CancellationToken);
    }
}
