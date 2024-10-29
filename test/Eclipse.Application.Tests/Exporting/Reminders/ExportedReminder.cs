namespace Eclipse.Application.Tests.Exporting.Reminders;

internal class ExportedReminder : ExportedRow
{
    public Guid UserId { get; set; }
    public string? Text { get; set; }
    public string? NotifyAt { get; set; }
}
