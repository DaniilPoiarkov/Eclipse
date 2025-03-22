using Eclipse.Application.Contracts.Exporting;

namespace Eclipse.Application.Exporting.Users;

public sealed class ImportUserDto : ImportEntityBase
{
    public string Name { get; set; } = string.Empty;

    public string Surname { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public long ChatId { get; set; }

    public string Culture { get; set; } = string.Empty;

    public bool NotificationsEnabled { get; set; }
    public bool IsEnabled { get; set; }

    public string Gmt { get; set; } = string.Empty;
}
