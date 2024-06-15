using Eclipse.Domain.Shared.Importing;

namespace Eclipse.Domain.Users.Import;

public class ImportReminderDto : ImportEntityBase
{
    public Guid UserId { get; set; }

    public string Text { get; set; } = string.Empty;

    public string NotifyAt { get; set; } = string.Empty;
}
