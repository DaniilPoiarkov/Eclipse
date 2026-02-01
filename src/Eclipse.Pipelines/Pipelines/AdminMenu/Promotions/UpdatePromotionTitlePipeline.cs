using Eclipse.Application.Contracts.Promotions;
using Eclipse.Common.Caching;
using Eclipse.Core.Context;
using Eclipse.Core.Results;
using Eclipse.Core.Routing;
using Eclipse.Pipelines.Stores.Messages;

using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.AdminMenu.Promotions;

[Route("", "/admin_promotions_update_title")]
internal sealed class UpdatePromotionTitlePipeline : AdminPipelineBase
{
    private readonly IPromotionService _promotionService;

    private readonly IMessageStore _messageStore;

    private readonly ICacheService _cacheService;

    public UpdatePromotionTitlePipeline(
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
        RegisterStage(RequestNewTitle);
        RegisterStage(UpdatePromotionTitle);
    }

    private async Task<IResult> RequestNewTitle(MessageContext context, CancellationToken cancellationToken)
    {
        var message = await _messageStore.GetOrDefaultAsync(
            new MessageKey(context.ChatId),
            cancellationToken
        );

        return MenuAndClearPrevious(new ReplyKeyboardRemove(), message, Localizer["Pipelines:Admin:Promotions:UpdateTitle:Request"]);
    }

    private async Task<IResult> UpdatePromotionTitle(MessageContext context, CancellationToken cancellationToken)
    {
        if (context.Value.Equals("/cancel"))
        {
            return Menu(PromotionsButtons, Localizer["Pipelines:Admin:Promotions:UpdateTitle:Cancelled"]);
        }

        if (context.Value.IsNullOrEmpty())
        {
            return Menu(PromotionsButtons, Localizer["Pipelines:Admin:Promotions:UpdateTitle:Invalid"]);
        }

        var promotionId = await _cacheService.GetOrCreateAsync(
            $"admin-promotions-promotion-{context.ChatId}",
            () => Task.FromResult(Guid.Empty),
            cancellationToken: cancellationToken
        );

        var result = await _promotionService.Update(
            promotionId,
            new UpdatePromotionRequest(context.Value),
            cancellationToken
        );

        var message = result.IsSuccess
            ? Localizer["Pipelines:Admin:Promotions:UpdateTitle:Success"]
            : Localizer["Pipelines:Admin:Promotions:UpdateTitle:Failure{Reason}", result.Error.Description];

        return Menu(PromotionsButtons, message);
    }
}
