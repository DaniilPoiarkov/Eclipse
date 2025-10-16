using Eclipse.Application.Contracts.Reports;

namespace Eclipse.Application.MoodRecords.Report
{
    internal interface IMoodReportSender
    {
        Task Send(Guid userId, MoodReportOptions options, CancellationToken cancellationToken);
    }
}