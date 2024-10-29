namespace Eclipse.Application.Tests.Exporting.Users;

internal class ExportedUser : ExportedRow
{
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Culture { get; set; } = string.Empty;
    public string Gmt { get; set; } = string.Empty;
    public long ChatId { get; set; }
    public bool NotificationsEnabled { get; set; }
}
