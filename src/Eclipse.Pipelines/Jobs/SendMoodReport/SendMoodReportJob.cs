using Eclipse.Application.Reports;
using Eclipse.Common.Clock;
using Eclipse.Common.Plots;
using Eclipse.Domain.MoodRecords;
using Eclipse.Domain.Shared.MoodRecords;
using Eclipse.Domain.Users;
using Eclipse.Localization.Culture;
using Eclipse.Localization.Extensions;

using Microsoft.Extensions.Localization;

using Quartz;

using Telegram.Bot;
using Telegram.Bot.Types;

using User = Eclipse.Domain.Users.User;

namespace Eclipse.Pipelines.Jobs.SendMoodReport;

internal sealed class SendMoodReportJob : EclipseJobBase
{
    private static readonly TimeOnly _time = new(19, 0);

    private static readonly TimeOnly _recordTime = new(0, 0);

    private static readonly int _width = 550;
    
    private static readonly int _height = 300;


    private readonly IUserRepository _userRepository;

    private readonly IMoodRecordRepository _moodRecordRepository;

    private readonly ITelegramBotClient _client;

    private readonly IStringLocalizer<SendMoodReportJob> _localizer;

    private readonly ITimeProvider _timeProvider;

    private readonly ICurrentCulture _currentCulture;

    private readonly IPlotGenerator _plotGenerator;

    public SendMoodReportJob(
        IUserRepository userRepository,
        IMoodRecordRepository moodRecordRepository,
        ITelegramBotClient client,
        IStringLocalizer<SendMoodReportJob> localizer,
        ITimeProvider timeProvider,
        ICurrentCulture currentCulture,
        IPlotGenerator plotGenerator)
    {
        _userRepository = userRepository;
        _moodRecordRepository = moodRecordRepository;
        _client = client;
        _localizer = localizer;
        _timeProvider = timeProvider;
        _currentCulture = currentCulture;
        _plotGenerator = plotGenerator;
    }

    public override async Task Execute(IJobExecutionContext context)
    {
        var date = _timeProvider.Now;

        if (date.DayOfWeek != DayOfWeek.Sunday)
        {
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

        var sendings = new List<Task>(users.Count);

        foreach (var pair in pairs)
        {
            var user = pair.User;
            var options = pair.Options;

            using var _ = _currentCulture.UsingCulture(user.Culture);
            _localizer.UseCurrentCulture(_currentCulture);

            var message = _localizer["Jobs:SendMoodReport:Caption"].Value;

            sendings.Add(
                SendReportAsync(context, user, options, message)
            );
        }

        await Task.WhenAll(sendings);
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

    private async Task SendReportAsync(IJobExecutionContext context, User user, PlotOptions<DateTime, int> options, string message)
    {
        using var stream = _plotGenerator.Create(options);

        await _client.SendPhotoAsync(user.ChatId,
            InputFile.FromStream(stream, $"mood-report.png"),
            caption: message,
            cancellationToken: context.CancellationToken
        );
    }

    private static async Task RescheduleJobAsync(IJobExecutionContext context, DateTime date)
    {
        var scheduler = context.Scheduler;

        await scheduler.PauseJob(context.JobDetail.Key, context.CancellationToken);

        var trigger = TriggerBuilder.Create()
            .ForJob(context.JobDetail.Key)
            .StartAt(date.NextDayOfWeek(DayOfWeek.Sunday))
            .WithSimpleSchedule(schedule => schedule
                .WithIntervalInMinutes(1)
                .RepeatForever())
            .Build();

        await scheduler.RescheduleJob(context.Trigger.Key, trigger, context.CancellationToken);
    }
}
