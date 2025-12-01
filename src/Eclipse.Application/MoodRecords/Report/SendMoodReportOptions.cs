namespace Eclipse.Application.MoodRecords.Report;

internal sealed record SendMoodReportOptions(
    DateTime From,
    DateTime To,
    string Message
);
