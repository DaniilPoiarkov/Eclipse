using Eclipse.Domain.Users;

namespace Eclipse.Application.Exporting.Reminders;

public sealed class ImportRemindersValidationOptions
{
    public List<User> Users { get; set; } = [];
}
