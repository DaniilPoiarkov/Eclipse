namespace Eclipse.Application.Contracts.Reports;

public interface IReportsService
{
    Task<MemoryStream> GetMoodReportAsync(Guid userId, MoodReportOptions options, CancellationToken cancellationToken = default);
}
