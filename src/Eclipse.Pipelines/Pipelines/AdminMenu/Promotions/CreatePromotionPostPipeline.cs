using Eclipse.Application.Contracts.Promotions;
using Eclipse.Common.Caching;
using Eclipse.Core.Context;
using Eclipse.Core.Results;
using Eclipse.Core.Routing;
using Eclipse.Domain.Promotions;
using Eclipse.Localization.Localizers;
using Eclipse.Pipelines.Caching;
using Eclipse.Pipelines.Stores.Messages;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.AdminMenu.Promotions;

[Route("Menu:AdminMenu:Promotions:Create", "/admin_promotions_create")]
internal sealed class CreatePromotionPostPipeline : AdminPipelineBase
{
    private readonly ICacheService _cacheService;

    private readonly ITelegramBotClient _botClient;

    private readonly IPromotionService _promotionService;

    private readonly IMessageStore _messageStore;

    public CreatePromotionPostPipeline(
        ICacheService cacheService,
        ITelegramBotClient botClient,
        IPromotionService promotionService,
        IMessageStore messageStore)
    {
        _cacheService = cacheService;
        _botClient = botClient;
        _promotionService = promotionService;
        _messageStore = messageStore;
    }

    protected override void Initialize()
    {
        RegisterStage(RequestPostMessage);
        RegisterStage(ReviewPostMessageAsync);
        RegisterStage(SetPromotionTitle);
        RegisterStage(SavePromotionPostAsync);
    }

    private IResult RequestPostMessage(MessageContext context)
    {
        return Menu(new ReplyKeyboardRemove(), Localizer["Pipelines:Admin:Promotions:Create:Request"]);
    }

    private async Task<IResult> ReviewPostMessageAsync(MessageContext context, CancellationToken cancellationToken)
    {
        if (context.Value.Equals("/cancel"))
        {
            FinishPipeline();
            return Menu(PromotionsButtons, Localizer["Pipelines:Admin:Promotions:Create:Cancelled"]);
        }

        if (Update.Message is not { })
        {
            FinishPipeline();
            return Menu(PromotionsButtons, Localizer["Pipelines:Admin:Promotions:Create:Invalid"]);
        }

        var options = new CacheOptions
        {
            Expiration = CacheConsts.FiveMinutes
        };

        await _cacheService.SetAsync(
            $"promotions-create-message-{context.ChatId}",
            Update.Message.Id,
            options,
            cancellationToken
        );

        await _botClient.CopyMessage(context.ChatId, context.ChatId, Update.Message.Id, cancellationToken: cancellationToken);

        return Menu(
            [
                [ new KeyboardButton(Localizer["Pipelines:Admin:Promotions:Create:Review:Continue"]) ],
                [ new KeyboardButton(Localizer["Pipelines:Admin:Promotions:Create:Review:Cancel"]) ]
            ],
            Localizer["Pipelines:Admin:Promotions:Create:Review"]
        );
    }

    private async Task<IResult> SetPromotionTitle(MessageContext context, CancellationToken cancellationToken)
    {
        var message = await _messageStore.GetOrDefaultAsync(
            new MessageKey(context.ChatId),
            cancellationToken
        );

        if (!ContinuePromotionProcessing(context))
        {
            return MenuAndClearPrevious(PromotionsButtons, message, Localizer["Pipelines:Admin:Promotions:Create:Cancelled"]);
        }

        return MenuAndClearPrevious(new ReplyKeyboardRemove(), message, Localizer["Pipelines:Admin:Promotions:Create:Title"]);
    }

    private async Task<IResult> SavePromotionPostAsync(MessageContext context, CancellationToken cancellationToken)
    {
        if (context.Value.Equals("/cancel"))
        {
            return Menu(PromotionsButtons, Localizer["Pipelines:Admin:Promotions:Create:Cancelled"]);
        }

        if (context.Value.IsNullOrEmpty())
        {
            return Menu(PromotionsButtons, Localizer["Pipelines:Admin:Promotions:Create:Invalid"]);
        }

        var messageId = await _cacheService.GetOrCreateAsync(
            $"promotions-create-message-{context.ChatId}",
            () => Task.FromResult<int?>(null),
            cancellationToken: cancellationToken
        );

        if (!messageId.HasValue)
        {
            return Menu(PromotionsButtons, Localizer["Pipelines:Admin:Promotions:Create:MessageNotFound"]);
        }

        var promotion = await _promotionService.Create(
            new CreatePromotionRequest(context.Value, context.ChatId, messageId.GetValueOrDefault(), string.Empty, string.Empty),
            cancellationToken
        );

        await _cacheService.SetForThreeDaysAsync(
            $"admin-promotions-promotion-{context.ChatId}",
            promotion.Id,
            context.ChatId,
            cancellationToken: cancellationToken
        );

        return Redirect<PromotionActionsPipeline>(
            Menu(new ReplyKeyboardRemove(), Localizer["Pipelines:Admin:Promotions:Create:Success"])
        );
    }

    private bool ContinuePromotionProcessing(MessageContext context)
    {
        return Localizer.TryConvertToLocalizableString(context.Value, out var localized)
            && localized.Equals("Pipelines:Admin:Promotions:Create:Review:Continue");
    }
}
