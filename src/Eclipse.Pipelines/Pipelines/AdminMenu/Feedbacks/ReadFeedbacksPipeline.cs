using Eclipse.Application.Contracts.Feedbacks;
using Eclipse.Common.Caching;
using Eclipse.Common.Linq;
using Eclipse.Core.Context;
using Eclipse.Core.Results;
using Eclipse.Core.Routing;
using Eclipse.Localization.Localizers;
using Eclipse.Pipelines.Stores.Messages;

using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.AdminMenu.Feedbacks;

[Route("Menu:AdminMenu:Feedbacks:Read", "/admin_feedbacks_read")]
internal sealed class ReadFeedbacksPipeline : AdminPipelineBase
{
    private readonly IFeedbackService _feedbackService;

    private readonly ICacheService _cacheService;

    private readonly IMessageStore _messageStore;

    private static readonly int _pageSize = 15;

    public ReadFeedbacksPipeline(IFeedbackService feedbackService, ICacheService cacheService, IMessageStore messageStore)
    {
        _feedbackService = feedbackService;
        _cacheService = cacheService;
        _messageStore = messageStore;
    }

    protected override void Initialize()
    {
        RegisterStage(ReadFeedbacks);
        RegisterStage(HandleUpdate);
    }

    private async Task<IResult> ReadFeedbacks(MessageContext context, CancellationToken cancellationToken)
    {
        int page = await GetPageAsync(context, cancellationToken);

        var request = new PaginationRequest<GetFeedbacksOptions>
        {
            Page = page,
            PageSize = _pageSize,
            Options = new GetFeedbacksOptions()
        };

        var list = await _feedbackService.GetListAsync(request, cancellationToken);

        var text = GetMessage(request, list);
        var buttons = GetButtons(page, list);

        if (buttons.IsNullOrEmpty())
        {
            FinishPipeline();
        }

        return Menu(buttons, text);
    }

    private async Task<IResult> HandleUpdate(MessageContext context, CancellationToken cancellationToken)
    {
        var message = await _messageStore.GetOrDefaultAsync(
            new MessageKey(context.ChatId),
            cancellationToken
        );

        if (context.Value is not ("◀️" or "▶️"))
        {
            return InvalidActionOrRedirect(message, context);
        }

        int page = await GetPageAsync(context, cancellationToken);

        var request = new PaginationRequest<GetFeedbacksOptions>
        {
            Page = page,
            PageSize = _pageSize,
            Options = new GetFeedbacksOptions()
        };

        var list = await _feedbackService.GetListAsync(request, cancellationToken);

        var text = GetMessage(request, list);
        var buttons = GetButtons(page, list);

        RegisterStage(HandleUpdate);

        if (message is null || message.ReplyMarkup is null)
        {
            return Menu(buttons, text);
        }

        return Edit(message.Id, text, buttons);
    }

    private IResult InvalidActionOrRedirect(Message? message, MessageContext context)
    {
        if (context.Value == "go_back")
        {
            return MenuAndClearPrevious(FeedbacksButtons, message, Localizer["Okay"]);
        }

        try
        {
            var localized = Localizer.ToLocalizableString(context.Value);

            return localized switch
            {
                "Menu:AdminMenu:Feedbacks:Request" => RemoveMenuAndRedirect<RequestFeedbackPipeline>(message),
                "Menu:AdminMenu:Feedbacks:Read" => RemoveMenuAndRedirect<ReadFeedbacksPipeline>(message),
                "Menu:AdminMenu" => RemoveMenuAndRedirect<AdminModePipeline>(message),
                _ => MenuAndClearPrevious(FeedbacksButtons, message, Localizer["Error"])
            };
        }
        catch
        {
            return MenuAndClearPrevious(FeedbacksButtons, message, Localizer["Error"]);
        }
    }

    private async Task<int> GetPageAsync(MessageContext context, CancellationToken cancellationToken)
    {
        var page = await _cacheService.GetOrCreateAsync(
            $"pipelines-admin-feedbacks-read-{context.ChatId}",
            () => Task.FromResult(1),
            new CacheOptions { Expiration = CacheConsts.ThreeDays },
            cancellationToken
        );

        page = context.Value switch
        {
            "◀️" => page - 1,
            "▶️" => page + 1,
            _ => page
        };

        if (page <= 0)
        {
            page = 1;
        }

        await _cacheService.SetAsync(
            $"pipelines-admin-feedbacks-read-{context.ChatId}",
            page,
            new CacheOptions { Expiration = CacheConsts.ThreeDays },
            cancellationToken
        );

        return page;
    }

    private static string GetMessage(PaginationRequest<GetFeedbacksOptions> request, PaginatedList<FeedbackDto> list)
    {
        var skippedCount = request.GetSkipCount();

        var feedbacks = list.Items.Select((f, i) => $"{i + skippedCount + 1}. {f.Rate} | {f.Comment}")
            .Join(Environment.NewLine);

        return $"Page {request.Page} of {list.Pages}. Total cound: {list.TotalCount}.{Environment.NewLine}" +
            $"{Environment.NewLine}" +
            $"{feedbacks}";
    }

    private List<List<InlineKeyboardButton>> GetButtons(int page, PaginatedList<FeedbackDto> list)
    {
        List<List<InlineKeyboardButton>> buttons = [];

        if (page > 1 && page < list.Pages)
        {
            buttons.Add([InlineKeyboardButton.WithCallbackData("◀️"), InlineKeyboardButton.WithCallbackData("▶️")]);
            buttons.Add([InlineKeyboardButton.WithCallbackData(Localizer["GoBack"], "go_back")]);
        }
        if (page == 1 && list.Pages > 1)
        {
            buttons.Add([InlineKeyboardButton.WithCallbackData("▶️")]);
            buttons.Add([InlineKeyboardButton.WithCallbackData(Localizer["GoBack"], "go_back")]);
        }
        if (page == list.Pages && list.Pages > 1)
        {
            buttons.Add([InlineKeyboardButton.WithCallbackData("◀️")]);
            buttons.Add([InlineKeyboardButton.WithCallbackData(Localizer["GoBack"], "go_back")]);
        }

        return buttons;
    }
}
