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
        var time = date.GetTime();

        var users = (await _userRepository.GetByExpressionAsync(u => u.NotificationsEnabled, context.CancellationToken))
            .Where(u => time.Add(u.Gmt) == _time)
            .ToList();

        var after = date.PreviousDayOfWeek(DayOfWeek.Sunday);

        var moodRecords = await _moodRecordRepository.GetByExpressionAsync(
            mr => users.Select(u => u.Id).Contains(mr.UserId) && mr.CreatedAt >= after,
            context.CancellationToken
        );

        var groups = moodRecords.GroupBy(mr => mr.UserId);

        var sendings = new List<Task>(users.Count);

        foreach (var group in groups)
        {
            if (group.IsNullOrEmpty())
            {
                continue;
            }

            var user = users.FirstOrDefault(u => u.Id == group.Key);

            if (user is null)
            {
                continue;
            }

            var records = group
                .OrderBy(r => r.CreatedAt)
                .ToList();

            var days = new DateTime[records.Count];
            var states = new int[records.Count];

            for (int i = 0; i < records.Count; i++)
            {
                var record = records[i];

                days[i] = record.CreatedAt.WithTime(_recordTime);
                states[i] = GetMoodScore(record.State);
            }

            var options = new PlotOptions<DateTime, int>
            {
                Title = $"{days[0]:dd.MM}-{days[^1]:dd.MM}",
                YAxisTitle = "Score",
                Width = 550,
                Height = 300,
                Ys = states,
                Xs = days
            };

            sendings.Add(
                SendReportAsync(context, user, options)
            );
        }

        await Task.WhenAll(sendings);
    }

    private async Task SendReportAsync(IJobExecutionContext context, User user, PlotOptions<DateTime, int> options)
    {
        using var _ = _currentCulture.UsingCulture(user.Culture);
        _localizer.UseCurrentCulture(_currentCulture);

        var message = _localizer["Jobs:SendMoodReport:Caption"].Value;

        using var stream = _plotGenerator.Create(options);

        await _client.SendPhotoAsync(user.ChatId,
            InputFile.FromStream(stream, $"mood-report.png"),
            caption: message,
            cancellationToken: context.CancellationToken
        );
    }

    private static int GetMoodScore(MoodState state)
    {
        return state switch
        {
            MoodState.Good => 1,
            _ => 0,
        };
    }
}
