using Eclipse.Domain.Users;

namespace Eclipse.Application.Tests.Exporting.Users;

internal sealed class UserRowComparer : IExportRowComparer<User, ExportedUser>
{
    public bool Compare(User entity, ExportedUser row)
    {
        return entity.Id == row.Id
            && entity.Name == row.Name
            && entity.Surname == row.Surname
            && entity.Gmt == TimeSpan.Parse(row.Gmt)
            && entity.UserName == row.UserName
            && entity.ChatId == row.ChatId
            && entity.Culture == row.Culture
            && entity.NotificationsEnabled == row.NotificationsEnabled;
    }
}
