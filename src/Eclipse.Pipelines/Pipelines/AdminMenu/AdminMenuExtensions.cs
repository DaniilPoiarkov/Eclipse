using Eclipse.Application.Contracts.Feedbacks;
using Eclipse.Application.Contracts.Promotions;
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

    internal static string GetReportingView(this PromotionDto promotion)
    {
        if (!promotion.Title.IsNullOrEmpty())
        {
            return $"{promotion.Title}. {promotion.Status}, published {promotion.TimesPublished} times.";
        }

        return $"{promotion.Id}. {promotion.Status}, published {promotion.TimesPublished} times.";
    }

    internal static string GetButtonName(this PromotionDto promotion)
    {
        if (promotion.Title.IsNullOrEmpty())
        {
            return promotion.Id.ToString().Truncate(15) + "..";
        }

        if (promotion.Title.Length <= 15)
        {
            return promotion.Title;
        }

        return $"{promotion.Title.Truncate(15)}..";
    }
}
