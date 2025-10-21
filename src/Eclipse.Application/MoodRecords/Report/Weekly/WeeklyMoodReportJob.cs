using Eclipse.Application.Jobs;
using Eclipse.Common.Background;
using Eclipse.Common.Clock;

using Microsoft.Extensions.Logging;

namespace Eclipse.Application.MoodRecords.Report.Weekly;

internal sealed class WeeklyMoodReportJob : JobWithArgs<UserIdJobData>
{
    private readonly IMoodReportSender _sender;

    private readonly ITimeProvider _timeProvider;

    public WeeklyMoodReportJob(
        IMoodReportSender sender,
        ITimeProvider timeProvider,
        ILogger<WeeklyMoodReportJob> logger) : base(logger)
    {
        _sender = sender;
        _timeProvider = timeProvider;
    }

    protected override async Task Execute(UserIdJobData args, CancellationToken cancellationToken)
    {
        var from = _timeProvider.Now.PreviousDayOfWeek(DayOfWeek.Sunday)
            .WithTime(0, 0);

        var options = new SendMoodReportOptions(from, _timeProvider.Now, "Jobs:MoodReport:Weekly:Caption");

        await _sender.Send(args.UserId, options, cancellationToken);
    }
}
