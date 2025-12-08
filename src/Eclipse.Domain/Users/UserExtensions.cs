namespace Eclipse.Domain.Users;

public static class UserExtensions
{
    public static string GetReportingDisplayName(this User user)
    {
        return user.UserName.IsNullOrWhiteSpace()
            ? $"{user.Name} {user.Surname} {user.ChatId}"
            : $"{user.UserName} {user.ChatId}";
    }
}
