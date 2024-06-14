using Eclipse.Domain.Shared.Importing;

namespace Eclipse.Domain.Users.Import;

public class ImportUserDto : ImportEntityBase
{
    public string Name { get; set; } = string.Empty;

    public string Surname { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public long ChatId { get; set; }

    public string Culture { get; set; } = string.Empty;

    public bool NotificationsEnabled { get; set; }

    public TimeSpan Gmt { get; set; }
}
