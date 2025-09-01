using Eclipse.Application.Contracts.Feedbacks;
using Eclipse.Application.Contracts.Users;
using Eclipse.Common.Caching;
using Eclipse.Core.Context;
using Eclipse.Core.Results;
using Eclipse.Core.Routing;
using Eclipse.Domain.Shared.Feedbacks;
using Eclipse.Pipelines.Stores.Messages;

using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.Common.Feedbacks;

[Route("Menu:Feedback", "/feedback")]
internal sealed class FeedbackPipeline : EclipsePipelineBase
{
    private readonly IFeedbackService _feedbackService;

    private readonly IMessageStore _messageStore;

    private readonly IUserService _userService;

    private readonly ICacheService _cacheService;

    private static readonly InlineKeyboardButton[] _buttons =
    [
        InlineKeyboardButton.WithCallbackData("1️⃣"),
        InlineKeyboardButton.WithCallbackData("2️⃣"),
        InlineKeyboardButton.WithCallbackData("3️⃣"),
        InlineKeyboardButton.WithCallbackData("4️⃣"),
        InlineKeyboardButton.WithCallbackData("5️⃣"),
    ];

    public FeedbackPipeline(
        IFeedbackService feedbackService,
        IMessageStore messageStore,
        IUserService userService,
        ICacheService cacheService
    )
    {
        _feedbackService = feedbackService;
        _messageStore = messageStore;
        _userService = userService;
        _cacheService = cacheService;
    }

    protected override void Initialize()
    {
        RegisterStage(AskToRateExperience);
        RegisterStage(AskForComment);
        RegisterStage(SaveFeedback);
    }

    private async Task<IResult> AskToRateExperience(MessageContext context, CancellationToken cancellationToken = default)
    {
        var message = await _messageStore.GetOrDefaultAsync(new MessageKey(context.ChatId), cancellationToken);

        return EditedOrDefaultResult(message, Menu(_buttons, Localizer["Pipelines:Feedback:Rate"]));
    }

    private async Task<IResult> AskForComment(MessageContext context, CancellationToken cancellationToken = default)
    {
        if (context.Value.EqualsCurrentCultureIgnoreCase("/cancel"))
        {
            FinishPipeline();
            return Menu(MainMenuButtons, Localizer["Okay"]);
        }

        var rate = context.Value switch
        {
            "5️⃣" => new FeedbackAnswer(FeedbackRate.Excellent, "Pipelines:Feedback:Send:Good"),
            "4️⃣" => new FeedbackAnswer(FeedbackRate.Good, "Pipelines:Feedback:Send:Good"),
            "3️⃣" => new FeedbackAnswer(FeedbackRate.Normal, "Pipelines:Feedback:Send:Bad"),
            "2️⃣" => new FeedbackAnswer(FeedbackRate.Bad, "Pipelines:Feedback:Send:Bad"),
            "1️⃣" => new FeedbackAnswer(FeedbackRate.Awful, "Pipelines:Feedback:Send:Bad"),
            _ => new FeedbackAnswer(null, "Pipelines:Feedback:Send:NotDefined")
        };

        var message = await _messageStore.GetOrDefaultAsync(new MessageKey(context.ChatId), cancellationToken);

        if (rate.Rate is null)
        {
            FinishPipeline();
            RegisterStage(AskForComment);
            RegisterStage(SaveFeedback);
            return EditedOrDefaultResult(message, Menu(_buttons, Localizer[rate.Message]));
        }

        await _cacheService.SetAsync(
            $"feedback-rate-{context.ChatId}",
            rate.Rate,
            new CacheOptions { Expiration = CacheConsts.OneDay },
            cancellationToken
        );

        return EditedOrDefaultResult(message, Menu(new ReplyKeyboardRemove(), Localizer[rate.Message]));
    }

    private async Task<IResult> SaveFeedback(MessageContext context, CancellationToken cancellationToken = default)
    {
        if (context.Value.EqualsCurrentCultureIgnoreCase("/cancel"))
        {
            return Menu(MainMenuButtons, Localizer["Okay"]);
        }

        if (context.Value.IsNullOrEmpty())
        {
            RegisterStage(SaveFeedback);
            return Text(Localizer["Pipelines:Feedback:Send:Empty"]);
        }

        var rate = await _cacheService.GetOrCreateAsync(
            $"feedback-rate-{context.ChatId}",
            () => Task.FromResult<FeedbackRate?>(null),
            new CacheOptions { Expiration = TimeSpan.FromMinutes(10) },
            cancellationToken
        );

        if (rate is null)
        {
            return Menu(MainMenuButtons, Localizer["Pipelines:Feedback:Error"]);
        }

        var user = await _userService.GetByChatIdAsync(context.ChatId, cancellationToken);

        if (!user.IsSuccess)
        {
            return Menu(MainMenuButtons, Localizer["Pipelines:Feedback:Error"]);
        }

        await _feedbackService.CreateAsync(user.Value.Id,
            new CreateFeedbackModel(context.Value, rate.Value),
            cancellationToken
        );

        await _cacheService.DeleteAsync($"feedback-rate-{context.ChatId}", cancellationToken);

        return Menu(MainMenuButtons, Localizer["Pipelines:Feedback:Thanks"]);
    }

    private static IResult EditedOrDefaultResult(Message? message, IResult @default)
    {
        return message is not { ReplyMarkup: { } }
            ? @default
            : Multiple(
                Edit(message.MessageId, InlineKeyboardMarkup.Empty()),
                @default
            );
    }
}
