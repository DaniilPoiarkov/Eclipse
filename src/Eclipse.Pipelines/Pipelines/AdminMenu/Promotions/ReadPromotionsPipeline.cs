using Eclipse.Application.Contracts.Promotions;
using Eclipse.Common.Caching;
using Eclipse.Common.Linq;
using Eclipse.Core.Context;
using Eclipse.Core.Results;
using Eclipse.Core.Routing;
using Eclipse.Localization.Localizers;
using Eclipse.Pipelines.Caching;
using Eclipse.Pipelines.Pipelines.AdminMenu.Feedbacks;
using Eclipse.Pipelines.Stores.Messages;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.AdminMenu.Promotions;

[Route("Menu:AdminMenu:Promotions:Read", "/admin_promotions_read")]
internal sealed class ReadPromotionsPipeline : AdminPipelineBase
{
    private readonly IPromotionService _promotionService;

    private readonly ICacheService _cacheService;

    private readonly IMessageStore _messageStore;

    private readonly ITelegramBotClient _botClient;

    private static readonly int _pageSize = 2;

    public ReadPromotionsPipeline(
        IPromotionService promotionService,
        ICacheService cacheService,
        IMessageStore messageStore,
        ITelegramBotClient botClient)
    {
        _promotionService = promotionService;
        _cacheService = cacheService;
        _messageStore = messageStore;
        _botClient = botClient;
    }

    protected override void Initialize()
    {
        RegisterStage(ReadPromotions);
        RegisterStage(HandleUpdate);
    }

    private async Task<IResult> ReadPromotions(MessageContext context, CancellationToken cancellationToken)
    {
        int page = await _cacheService.GetListPageAsync($"pipelines-admin-feedbacks-read-{context.ChatId}", context.Value, cancellationToken);

        var request = new PaginationRequest<GetPromotionsOptions>
        {
            Page = page,
            PageSize = _pageSize,
            Options = new GetPromotionsOptions()
        };

        var promotions = await _promotionService.GetList(request, cancellationToken);

        var text = GetMessage(request, promotions);

        var buttons = promotions.Items
            .Select(p => InlineKeyboardButton.WithCallbackData($"{p.Id.ToString().Truncate(5)}..", p.Id.ToString()))
            .Chunk(2)
            .Concat(
                GetPagingButtons(page, promotions)
            );

        if (buttons.IsNullOrEmpty())
        {
            FinishPipeline();
        }

        return Menu(new InlineKeyboardMarkup(buttons), text);
    }

    private async Task<IResult> HandleUpdate(MessageContext context, CancellationToken cancellationToken)
    {
        var message = await _messageStore.GetOrDefaultAsync(
            new MessageKey(context.ChatId),
            cancellationToken
        );

        var promotionId = context.Value.ToGuid();

        if (promotionId != Guid.Empty)
        {
            return await PreviewPromotion(context, message, promotionId, cancellationToken);
        }

        if (context.Value is not ("◀️" or "▶️"))
        {
            return InvalidActionOrRedirect(message, context);
        }

        return await SendNextPage(context, message, cancellationToken);
    }

    private async Task<IResult> PreviewPromotion(MessageContext context, Message? message, Guid promotionId, CancellationToken cancellationToken)
    {
        var promotion = await _promotionService.Find(promotionId, cancellationToken);

        if (!promotion.IsSuccess)
        {
            return MenuAndClearPrevious(PromotionsButtons, message, Localizer["{0}NotFound", "Promotion"]);
        }

        await _botClient.CopyMessage(context.ChatId, promotion.Value.FromChatId, promotion.Value.MessageId, cancellationToken: cancellationToken);

        // TODO: Localize message and send buttons [Publish] and [Cancel] with according further actions.
        return MenuAndClearPrevious(PromotionsButtons, message, Localizer["Selected promotion. Wanna publish?"]);
    }

    private async Task<IResult> SendNextPage(MessageContext context, Message? message, CancellationToken cancellationToken)
    {
        int page = await _cacheService.GetListPageAsync($"pipelines-admin-feedbacks-read-{context.ChatId}", context.Value, cancellationToken);

        var request = new PaginationRequest<GetPromotionsOptions>
        {
            Page = page,
            PageSize = _pageSize,
            Options = new GetPromotionsOptions()
        };

        var promotions = await _promotionService.GetList(request, cancellationToken);

        var text = GetMessage(request, promotions);

        var buttons = promotions.Items
            .Select(p => InlineKeyboardButton.WithCallbackData($"{p.Id.ToString().Truncate(5)}..", p.Id.ToString()))
            .Chunk(2)
            .Concat(
                GetPagingButtons(page, promotions)
            );

        RegisterStage(HandleUpdate);

        if (message is not { ReplyMarkup: { } })
        {
            return Menu(new InlineKeyboardMarkup(buttons), text);
        }

        return Edit(message.Id, text, new InlineKeyboardMarkup(buttons));
    }

    private IResult InvalidActionOrRedirect(Message? message, MessageContext context)
    {
        if (context.Value == "go_back")
        {
            return MenuAndClearPrevious(PromotionsButtons, message, Localizer["Okay"]);
        }

        try
        {
            var localized = Localizer.ToLocalizableString(context.Value);

            return localized switch
            {
                "Menu:AdminMenu:Promotions:Post" => RemoveMenuAndRedirect<RequestFeedbackPipeline>(message),
                "Menu:AdminMenu:Promotions:Read" => RemoveMenuAndRedirect<ReadFeedbacksPipeline>(message),
                "Menu:AdminMenu" => RemoveMenuAndRedirect<AdminModePipeline>(message),
                _ => MenuAndClearPrevious(PromotionsButtons, message, Localizer["Error"])
            };
        }
        catch
        {
            return MenuAndClearPrevious(PromotionsButtons, message, Localizer["Error"]);
        }
    }

    private string GetMessage(PaginationRequest<GetPromotionsOptions> request, PaginatedList<PromotionDto> list)
    {
        var skippedCount = request.GetSkipCount();

        // TODO: Promotion should have human friendly representation.
        var promotions = list.Items.Select((p, i) => $"{i + skippedCount + 1}. {p.Id}. {p.Status}, published {p.TimesPublished} times.")
            .Join(Environment.NewLine);

        return GetPagingMessage(request.Page, list.Pages, list.TotalCount, promotions);
    }
}
