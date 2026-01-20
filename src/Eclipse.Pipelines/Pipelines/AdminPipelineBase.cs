using Eclipse.Pipelines.Validation;

using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines;

[AdminOnly]
public abstract class AdminPipelineBase : EclipsePipelineBase
{
    protected IReadOnlyCollection<IReadOnlyCollection<KeyboardButton>> AdminMenuButtons => new List<KeyboardButton[]>
    {
        new[] { new KeyboardButton(Localizer["Menu:Admin:View"]), new KeyboardButton(Localizer["Menu:Admin:Send"]) },
        new[] { new KeyboardButton(Localizer["Menu:Admin:Export"]), new KeyboardButton(Localizer["Menu:Admin:Promotions"]) },
        new[] { new KeyboardButton(Localizer["Menu:Admin:Feedbacks"]) },
        new[] { new KeyboardButton(Localizer["Menu:Admin:SwitchToUserMode"]) },
    };

    protected IReadOnlyCollection<IReadOnlyCollection<KeyboardButton>> SendButtons => new List<KeyboardButton[]>()
    {
        new[] { new KeyboardButton(Localizer["Menu:Admin:Send:User"]), new KeyboardButton(Localizer["Menu:Admin:Send:All"]) },
        new[] { new KeyboardButton(Localizer["Menu:Admin"]) }
    };

    protected IReadOnlyCollection<IReadOnlyCollection<KeyboardButton>> ViewButtons => new List<KeyboardButton[]>()
    {
        new[] { new KeyboardButton(Localizer["Menu:Admin:View:Suggestions"]), new KeyboardButton(Localizer["Menu:Admin:View:Users"]) },
        new[] { new KeyboardButton(Localizer["Menu:Admin"]) }
    };

    protected IReadOnlyCollection<IReadOnlyCollection<KeyboardButton>> ExportButtons => new List<KeyboardButton[]>()
    {
        new[] { new KeyboardButton(Localizer["Menu:Admin:Export:Users"]) },
        new[] { new KeyboardButton(Localizer["Menu:Admin:Export:TodoItems"]) },
        new[] { new KeyboardButton(Localizer["Menu:Admin:Export:Reminders"]) },
        new[] { new KeyboardButton(Localizer["Menu:Admin"]) }
    };

    protected IReadOnlyCollection<IReadOnlyCollection<KeyboardButton>> PromotionsButtons => new List<KeyboardButton[]>()
    {
        new[] { new KeyboardButton(Localizer["Menu:Admin:Promotions:Create"]), new KeyboardButton(Localizer["Menu:Admin:Promotions:Read"]) },
        new[] { new KeyboardButton(Localizer["Menu:Admin"]) }
    };

    protected IReadOnlyCollection<IReadOnlyCollection<KeyboardButton>> FeedbacksButtons => new List<KeyboardButton[]>()
    {
        new[] { new KeyboardButton(Localizer["Menu:Admin:Feedbacks:Read"]), new KeyboardButton(Localizer["Menu:Admin:Feedbacks:Request"]) },
        new[] { new KeyboardButton(Localizer["Menu:Admin"]) }
    };
}
