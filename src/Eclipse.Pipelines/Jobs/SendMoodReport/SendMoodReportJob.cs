using Eclipse.Common.Clock;
using Eclipse.Domain.MoodRecords;
using Eclipse.Domain.Users;
using Eclipse.Localization.Culture;

using Microsoft.Extensions.Localization;

using Quartz;

using Telegram.Bot;

namespace Eclipse.Pipelines.Jobs.SendMoodReport;

internal sealed class SendMoodReportJob : EclipseJobBase
{
    private static readonly TimeOnly _time = new(19, 0);

    private readonly IUserRepository _userRepository;

    private readonly IMoodRecordRepository _moodRecordRepository;

    private readonly ITelegramBotClient _client;

    private readonly IStringLocalizer<SendMoodReportJob> _localizer;

    private readonly ITimeProvider _timeProvider;

    private readonly ICurrentCulture _currentCulture;

    public SendMoodReportJob(
        IUserRepository userRepository,
        IMoodRecordRepository moodRecordRepository,
        ITelegramBotClient client,
        IStringLocalizer<SendMoodReportJob> localizer,
        ITimeProvider timeProvider,
        ICurrentCulture currentCulture)
    {
        _userRepository = userRepository;
        _moodRecordRepository = moodRecordRepository;
        _client = client;
        _localizer = localizer;
        _timeProvider = timeProvider;
        _currentCulture = currentCulture;
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

        var grouping = moodRecords.GroupBy(mr => mr.UserId);

        var sendings = new List<Task>(users.Count);

        foreach (var group in grouping)
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

            using var _ = _currentCulture.UsingCulture(user.Culture);

            var message = group
                .Select(mr => $"{mr.CreatedAt:dd.MM HH.mm} you felt {mr.State}")
                .Join(Environment.NewLine);

            sendings.Add(
                _client.SendTextMessageAsync(user.ChatId, message, cancellationToken: context.CancellationToken)
            );
        }

        await Task.WhenAll(sendings);
    }
}
