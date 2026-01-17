using Eclipse.Application.Contracts.Promotions;
using Eclipse.Common.Caching;
using Eclipse.Core.Context;
using Eclipse.Core.Results;
using Eclipse.Core.Routing;
using Eclipse.Domain.Promotions;
using Eclipse.Pipelines.Stores.Messages;

using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.AdminMenu.Promotions;

[Route("", "/admin_promotions_publish")]
internal sealed class PublishPromotionPipeline : AdminPipelineBase
{
    private readonly IPromotionService _promotionService;

    private readonly IMessageStore _messageStore;

    private readonly ICacheService _cacheService;

    public PublishPromotionPipeline(
        IPromotionService promotionService,
        IMessageStore messageStore,
        ICacheService cacheService)
    {
        _promotionService = promotionService;
        _messageStore = messageStore;
        _cacheService = cacheService;
    }

    protected override void Initialize()
    {
        RegisterStage(SendConfirmationCodeAsync);
        RegisterStage(PublishPromotion);
    }

    private async Task<IResult> SendConfirmationCodeAsync(MessageContext context, CancellationToken cancellationToken)
    {
        var message = await _messageStore.GetOrDefaultAsync(
            new MessageKey(context.ChatId),
            cancellationToken
        );

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
        if (context.Value.Equals("/cancel"))
        {
            FinishPipeline();
            return Menu(PromotionsButtons, Localizer["Pipelines:Admin:Promotions:Publish:Cancelled"]);
        }

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

        var promotionId = await _cacheService.GetOrCreateAsync(
            $"admin-promotions-promotion-{context.ChatId}",
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
}
