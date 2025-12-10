namespace Eclipse.Application.Contracts.MoodRecords;

public interface IMoodReportService
{
    Task<MemoryStream> GetAsync(Guid userId, MoodReportOptions options, CancellationToken cancellationToken = default);
}
