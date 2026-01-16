using Eclipse.Application.Contracts.Promotions;
using Eclipse.Common.Caching;
using Eclipse.Core.Context;
using Eclipse.Core.Results;
using Eclipse.Core.Routing;
using Eclipse.Localization.Localizers;
using Eclipse.Pipelines.Stores.Messages;

using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.AdminMenu.Promotions;

[Route("", "/admin_promotions_actions")]
internal sealed class PromotionActionsPipeline : AdminPipelineBase
{
    private readonly IPromotionService _promotionService;

    private readonly IMessageStore _messageStore;

    private readonly ICacheService _cacheService;

    private readonly ITelegramBotClient _botClient;

    public PromotionActionsPipeline(
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
        RegisterStage(HandleUpdate);
    }

    private async Task<IResult> ShowPromotion(MessageContext context, CancellationToken cancellationToken)
    {
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
            FinishPipeline();
            return Menu(PromotionsButtons, Localizer["Error"]);
        }

        var promotion = await _promotionService.Find(promotionId, cancellationToken);

        if (!promotion.IsSuccess)
        {
            FinishPipeline();
            return MenuAndClearPrevious(PromotionsButtons, message, Localizer["{0}NotFound", "Promotion"]);
        }

        List<IList<InlineKeyboardButton>> buttons = [
            [
                InlineKeyboardButton.WithCallbackData(Localizer["Pipelines:Admin:Promotions:Page:Publish"]),
                InlineKeyboardButton.WithCallbackData(Localizer["Pipelines:Admin:Promotions:Page:Delete"])
            ],
            [
                InlineKeyboardButton.WithCallbackData(Localizer["GoBack"], "go_back")
            ],
        ];

        await _botClient.CopyMessage(context.ChatId, promotion.Value.FromChatId, promotion.Value.MessageId, replyMarkup: new ReplyKeyboardRemove(), cancellationToken: cancellationToken);

        return MenuAndClearPrevious(new InlineKeyboardMarkup(buttons), message, Localizer["Pipelines:Admin:Promotions:Publish:Ask"]);
    }

    private async Task<IResult> HandleUpdate(MessageContext context, CancellationToken cancellationToken)
    {
        var message = await _messageStore.GetOrDefaultAsync(
            new MessageKey(context.ChatId),
            cancellationToken
        );

        if (context.Value.Equals("go_back"))
        {
            return MenuAndClearPrevious(PromotionsButtons, message, Localizer["Okay"]);
        }

        var localized = Localizer.ToLocalizableString(context.Value);

        return localized switch
        {
            "Pipelines:Admin:Promotions:Page:Publish" => Redirect<PublishPromotionPipeline>(),
            "Pipelines:Admin:Promotions:Page:Delete" => Redirect<DeletePromotionPipeline>(),
            _ => MenuAndClearPrevious(PromotionsButtons, message, Localizer["Error"])
        };
    }
}
