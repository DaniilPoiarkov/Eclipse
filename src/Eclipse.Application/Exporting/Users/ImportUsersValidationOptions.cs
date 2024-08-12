using Eclipse.Domain.Users;

namespace Eclipse.Application.Exporting.Users;

public sealed class ImportUsersValidationOptions
{
    internal List<User> Users { get; set; } = [];
}
