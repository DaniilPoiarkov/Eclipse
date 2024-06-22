namespace Eclipse.Application.Exporting.Models;

internal record ExportUserDto(Guid Id, string Name, string Surname, string UserName, long ChatId, string Culture, bool NotificationsEnabled, TimeSpan Gmt);
