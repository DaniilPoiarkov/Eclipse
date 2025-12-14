using Eclipse.Application.Contracts.Feedbacks;
using Eclipse.Application.Contracts.Users;

namespace Eclipse.Pipelines.Pipelines.AdminMenu;

internal static class AdminMenuExtensions
{
    internal static string GetDisplayName(this UserSlimDto user)
    {
        if (!user.UserName.IsNullOrEmpty())
        {
            return $"@{user.UserName} {user.ChatId}";
        }

        return $"{user.Name} {user.Surname} {user.ChatId}";
    }

    internal static string GetReportingView(this FeedbackDto feedback)
    {
        if (!feedback.Comment.IsNullOrEmpty())
        {
            return $"{feedback.Rate} | {feedback.Comment}";
        }

        return $"{feedback.Rate}";
    }
}
