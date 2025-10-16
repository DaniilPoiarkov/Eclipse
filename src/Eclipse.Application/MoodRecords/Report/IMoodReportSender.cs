namespace Eclipse.Application.MoodRecords.Report;

internal interface IMoodReportSender
{
    Task Send(Guid userId, SendMoodReportOptions options, CancellationToken cancellationToken = default);
}
