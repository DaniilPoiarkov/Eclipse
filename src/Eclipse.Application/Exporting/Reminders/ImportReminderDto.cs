using Eclipse.Application.Contracts.Exporting;

namespace Eclipse.Application.Exporting.Reminders;

public sealed class ImportReminderDto : ImportEntityBase
{
    public Guid UserId { get; set; }

    public string Text { get; set; } = string.Empty;

    public string NotifyAt { get; set; } = string.Empty;
}
