using Eclipse.Application.Contracts.Promotions;
using Eclipse.Common.Caching;
using Eclipse.Core.Context;
using Eclipse.Core.Results;
using Eclipse.Core.Routing;
using Eclipse.Domain.Promotions;
using Eclipse.Localization.Localizers;
using Eclipse.Pipelines.Stores.Messages;

using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.AdminMenu.Promotions;

[Route("Menu:AdminMenu:Promotions:Publish", "/admin_promotions_publish")]
internal sealed class PublishPromotionPipeline : AdminPipelineBase
{
    private readonly IPromotionService _promotionService;

    private readonly IMessageStore _messageStore;

    private readonly ICacheService _cacheService;

    private readonly ITelegramBotClient _botClient;

    public PublishPromotionPipeline(
        IPromotionService promotionService,
        IMessageStore messageStore,
        ICacheService cacheService,
        ITelegramBotClient botClient)
    {
        _promotionService = promotionService;
        _messageStore = messageStore;
        _cacheService = cacheService;
        _botClient = botClient;
    }

    protected override void Initialize()
    {
        RegisterStage(ShowPromotion);
        RegisterStage(SendConfirmationCodeAsync);
        RegisterStage(PublishPromotion);
    }

    private async Task<IResult> ShowPromotion(MessageContext context, CancellationToken cancellationToken)
    {
        var message = await _messageStore.GetOrDefaultAsync(
            new MessageKey(context.ChatId),
            cancellationToken
        );

        var promotionId = await _cacheService.GetOrCreateAsync(
            $"admin-promotions-publish-{context.ChatId}",
            () => Task.FromResult(Guid.Empty),
            cancellationToken: cancellationToken
        );

        if (promotionId == Guid.Empty)
        {
            FinishPipeline();
            return Menu(PromotionsButtons, Localizer["Error"]);
        }

        var promotion = await _promotionService.Find(promotionId, cancellationToken);

        if (!promotion.IsSuccess)
        {
            FinishPipeline();
            return MenuAndClearPrevious(PromotionsButtons, message, Localizer["{0}NotFound", "Promotion"]);
        }

        List<InlineKeyboardButton> buttons = [
            InlineKeyboardButton.WithCallbackData(Localizer["Pipelines:Admin:Promotions:Publish:Publish"]),
            InlineKeyboardButton.WithCallbackData(Localizer["GoBack"], "go_back"),
        ];

        await _botClient.CopyMessage(context.ChatId, promotion.Value.FromChatId, promotion.Value.MessageId, replyMarkup: new ReplyKeyboardRemove(), cancellationToken: cancellationToken);

        return MenuAndClearPrevious(new InlineKeyboardMarkup(buttons), message, Localizer["Pipelines:Admin:Promotions:Publish:Ask"]);
    }

    private async Task<IResult> SendConfirmationCodeAsync(MessageContext context, CancellationToken cancellationToken)
    {
        var message = await _messageStore.GetOrDefaultAsync(
            new MessageKey(context.ChatId),
            cancellationToken
        );

        if (context.Value == "go_back")
        {
            FinishPipeline();
            return MenuAndClearPrevious(PromotionsButtons, message, Localizer["Okay"]);
        }

        if (!ContinuePromotionProcessing(context))
        {
            FinishPipeline();
            return MenuAndClearPrevious(PromotionsButtons, message, Localizer["Pipelines:Admin:Promotions:Publish:Cancelled"]);
        }

        var confirmationCode = Enumerable.Range(0, 6)
            .Select(_ => Random.Shared.Next(0, 10))
            .Aggregate(string.Empty, (s, i) => $"{s}{i}");

        var options = new CacheOptions
        {
            Expiration = CacheConsts.FiveMinutes
        };

        await _cacheService.SetAsync(
            $"promotions-publish-confiramtion-code-{context.ChatId}",
            confirmationCode,
            options,
            cancellationToken
        );

        return MenuAndClearPrevious(new ReplyKeyboardRemove(), message, Localizer["Pipelines:Admin:Promotions:Publish:Confirm{0}", confirmationCode]);
    }

    private async Task<IResult> PublishPromotion(MessageContext context, CancellationToken cancellationToken)
    {
        var confirmationCode = await _cacheService.GetOrCreateAsync(
            $"promotions-publish-confiramtion-code-{context.ChatId}",
            () => Task.FromResult(string.Empty),
            cancellationToken: cancellationToken
        );

        if (!context.Value.Equals(confirmationCode))
        {
            return Menu(PromotionsButtons, Localizer["Pipelines:Admin:Promotions:Publish:ConfirmationFailed"]);
        }

        var message = await _messageStore.GetOrDefaultAsync(
            new MessageKey(context.ChatId),
            cancellationToken
        );

        // TODO: Fix promotion id retrieval.
        var promotionId = await _cacheService.GetOrCreateAsync(
            $"admin-promotions-publish-{context.ChatId}",
            () => Task.FromResult(Guid.Empty),
            cancellationToken: cancellationToken
        );

        if (promotionId == Guid.Empty)
        {
            return MenuAndClearPrevious(PromotionsButtons, message, Localizer["Error"]);
        }

        var result = await _promotionService.Publish(promotionId, cancellationToken);

        var text = result.IsSuccess
            ? Localizer["Pipelines:Admin:Promotions:Publish:SuccessfullyPublished"]
            : Localizer["Pipelines:Admin:Promotions:Publish:FailedToPublish{Reason}", result.Error.Description];

        return MenuAndClearPrevious(PromotionsButtons, message, text);
    }

    private bool ContinuePromotionProcessing(MessageContext context)
    {
        return Localizer.TryConvertToLocalizableString(context.Value, out var localized)
            && localized.Equals("Pipelines:Admin:Promotions:Publish:Publish");
    }
}
