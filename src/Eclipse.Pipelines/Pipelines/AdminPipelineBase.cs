using Eclipse.Pipelines.Validation;

using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines;

[AdminOnly]
public abstract class AdminPipelineBase : EclipsePipelineBase
{
    protected IReadOnlyCollection<IReadOnlyCollection<KeyboardButton>> AdminMenuButtons => new List<KeyboardButton[]>
    {
        new[] { new KeyboardButton(Localizer["Menu:AdminMenu:View"]), new KeyboardButton(Localizer["Menu:AdminMenu:Send"]) },
        new[] { new KeyboardButton(Localizer["Menu:AdminMenu:Export"]), new KeyboardButton(Localizer["Menu:AdminMenu:Promotions"]) },
        new[] { new KeyboardButton(Localizer["Menu:AdminMenu:Feedbacks"]) },
        new[] { new KeyboardButton(Localizer["Menu:AdminMenu:SwitchToUserMode"]) },
    };

    protected IReadOnlyCollection<IReadOnlyCollection<KeyboardButton>> SendButtons => new List<KeyboardButton[]>()
    {
        new[] { new KeyboardButton(Localizer["Menu:AdminMenu:Send:User"]), new KeyboardButton(Localizer["Menu:AdminMenu:Send:All"]) },
        new[] { new KeyboardButton(Localizer["Menu:AdminMenu"]) }
    };

    protected IReadOnlyCollection<IReadOnlyCollection<KeyboardButton>> ViewButtons => new List<KeyboardButton[]>()
    {
        new[] { new KeyboardButton(Localizer["Menu:AdminMenu:View:Suggestions"]), new KeyboardButton(Localizer["Menu:AdminMenu:View:Users"]) },
        new[] { new KeyboardButton(Localizer["Menu:AdminMenu"]) }
    };

    protected IReadOnlyCollection<IReadOnlyCollection<KeyboardButton>> ExportButtons => new List<KeyboardButton[]>()
    {
        new[] { new KeyboardButton(Localizer["Menu:AdminMenu:Export:Users"]) },
        new[] { new KeyboardButton(Localizer["Menu:AdminMenu:Export:TodoItems"]) },
        new[] { new KeyboardButton(Localizer["Menu:AdminMenu:Export:Reminders"]) },
        new[] { new KeyboardButton(Localizer["Menu:AdminMenu"]) }
    };

    protected IReadOnlyCollection<IReadOnlyCollection<KeyboardButton>> PromotionsButtons => new List<KeyboardButton[]>()
    {
        new[] { new KeyboardButton(Localizer["Menu:AdminMenu:Promotions:Create"]), new KeyboardButton(Localizer["Menu:AdminMenu:Promotions:Read"]) },
        new[] { new KeyboardButton(Localizer["Menu:AdminMenu"]) }
    };

    protected IReadOnlyCollection<IReadOnlyCollection<KeyboardButton>> FeedbacksButtons => new List<KeyboardButton[]>()
    {
        new[] { new KeyboardButton(Localizer["Menu:AdminMenu:Feedbacks:Read"]), new KeyboardButton(Localizer["Menu:AdminMenu:Feedbacks:Request"]) },
        new[] { new KeyboardButton(Localizer["Menu:AdminMenu"]) }
    };
}
