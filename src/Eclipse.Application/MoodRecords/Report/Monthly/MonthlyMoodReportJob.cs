using Eclipse.Application.Jobs;
using Eclipse.Common.Background;
using Eclipse.Common.Clock;

using Microsoft.Extensions.Logging;

using Quartz;

namespace Eclipse.Application.MoodRecords.Report.Monthly;

internal sealed class MonthlyMoodReportJob : JobWithArgs<UserIdJobData>, IJobWithKey
{
    private readonly IMoodReportSender _sender;

    private readonly ITimeProvider _timeProvider;

    public static JobKey Key => new(nameof(MonthlyMoodReportJob), "mood-records");

    public MonthlyMoodReportJob(
        IMoodReportSender sender,
        ITimeProvider timeProvider,
        ILogger<MonthlyMoodReportJob> logger) : base(logger)
    {
        _sender = sender;
        _timeProvider = timeProvider;
    }

    protected override async Task Execute(UserIdJobData args, CancellationToken cancellationToken)
    {
        var options = new SendMoodReportOptions(
            _timeProvider.Now.FirstDayOfTheMonth(),
            _timeProvider.Now.WithTime(23, 59),
            "Jobs:MoodReport:Monthly:Caption"
        );

        await _sender.Send(args.UserId, options, cancellationToken);
    }
}
