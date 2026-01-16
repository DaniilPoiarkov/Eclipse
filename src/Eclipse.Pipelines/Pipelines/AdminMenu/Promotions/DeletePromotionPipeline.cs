using Eclipse.Application.Contracts.Promotions;
using Eclipse.Common.Caching;
using Eclipse.Core.Context;
using Eclipse.Core.Results;
using Eclipse.Core.Routing;
using Eclipse.Pipelines.Stores.Messages;

using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.AdminMenu.Promotions;

[Route("Menu:AdminMenu:Promotions:Delete", "/admin_promotions_delete")]
internal sealed class DeletePromotionPipeline : AdminPipelineBase
{
    private readonly IPromotionService _promotionService;

    private readonly ICacheService _cacheService;

    private readonly IMessageStore _messageStore;

    public DeletePromotionPipeline(IPromotionService promotionService, ICacheService cacheService, IMessageStore messageStore)
    {
        _promotionService = promotionService;
        _cacheService = cacheService;
        _messageStore = messageStore;
    }

    protected override void Initialize()
    {
        RegisterStage(SendConfirmationCodeAsync);
        RegisterStage(DeletePromotion);
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
            $"promotions-delete-confiramtion-code-{context.ChatId}",
            confirmationCode,
            options,
            cancellationToken
        );

        return MenuAndClearPrevious(new ReplyKeyboardRemove(), message, Localizer["Pipelines:Admin:Promotions:Delete:Confirm{0}", confirmationCode]);
    }

    private async Task<IResult> DeletePromotion(MessageContext context, CancellationToken cancellationToken)
    {
        var confirmationCode = await _cacheService.GetOrCreateAsync(
            $"promotions-delete-confiramtion-code-{context.ChatId}",
            () => Task.FromResult(string.Empty),
            cancellationToken: cancellationToken
        );

        if (!context.Value.Equals(confirmationCode))
        {
            return Menu(PromotionsButtons, Localizer["Pipelines:Admin:Promotions:Delete:ConfirmationFailed"]);
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

        var result = await _promotionService.Delete(promotionId, cancellationToken);

        var text = result.IsSuccess
            ? Localizer["Pipelines:Admin:Promotions:Delete:Success"]
            : Localizer["Pipelines:Admin:Promotions:Delete:Failed{Reason}", result.Error.Description];

        return MenuAndClearPrevious(PromotionsButtons, message, text);
    }
}
