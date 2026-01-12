using Eclipse.Application.Contracts.Feedbacks;
using Eclipse.Application.Contracts.Users;
using Eclipse.Common.Caching;
using Eclipse.Common.Linq;
using Eclipse.Core.Context;
using Eclipse.Core.Results;
using Eclipse.Core.Routing;
using Eclipse.Localization.Localizers;
using Eclipse.Pipelines.Caching;
using Eclipse.Pipelines.Stores.Messages;

using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.AdminMenu.Feedbacks;

[Route("Menu:AdminMenu:Feedbacks:Read", "/admin_feedbacks_read")]
internal sealed class ReadFeedbacksPipeline : AdminPipelineBase
{
    private readonly IFeedbackService _feedbackService;

    private readonly IUserService _userService;

    private readonly ICacheService _cacheService;

    private readonly IMessageStore _messageStore;

    private static readonly int _pageSize = 15;

    public ReadFeedbacksPipeline(
        IFeedbackService feedbackService,
        IUserService userService,
        ICacheService cacheService,
        IMessageStore messageStore)
    {
        _feedbackService = feedbackService;
        _userService = userService;
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
        int page = await _cacheService.GetListPageAsync($"pipelines-admin-feedbacks-read-{context.ChatId}", context.Value, cancellationToken);

        var request = new PaginationRequest<GetFeedbacksOptions>
        {
            Page = page,
            PageSize = _pageSize,
            Options = new GetFeedbacksOptions()
        };

        var list = await _feedbackService.GetListAsync(request, cancellationToken);
        var users = await GetUsers(list, cancellationToken);

        var text = GetMessage(request, list, users.Items);
        var buttons = GetPagingButtons(page, list);

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

        if (context.Value is not ("◀️" or "▶️"))
        {
            return InvalidActionOrRedirect(message, context);
        }

        int page = await _cacheService.GetListPageAsync($"pipelines-admin-feedbacks-read-{context.ChatId}", context.Value, cancellationToken);

        var request = new PaginationRequest<GetFeedbacksOptions>
        {
            Page = page,
            PageSize = _pageSize,
            Options = new GetFeedbacksOptions()
        };

        var feedbacks = await _feedbackService.GetListAsync(request, cancellationToken);
        var users = await GetUsers(feedbacks, cancellationToken);

        var text = GetMessage(request, feedbacks, users.Items);
        var buttons = GetPagingButtons(page, feedbacks);

        RegisterStage(HandleUpdate);

        if (message is null || message.ReplyMarkup is null)
        {
            return Menu(new InlineKeyboardMarkup(buttons), text);
        }

        return Edit(message.Id, text, new InlineKeyboardMarkup(buttons));
    }

    private Task<PaginatedList<UserSlimDto>> GetUsers(PaginatedList<FeedbackDto> list, CancellationToken cancellationToken)
    {
        return _userService.GetListAsync(new PaginationRequest<GetUsersRequest>
        {
            Page = 1,
            PageSize = int.MaxValue,
            Options = new GetUsersRequest
            {
                UserIds = list.Items.Select(f => f.UserId).Distinct()
            }
        }, cancellationToken);
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

    private string GetMessage(PaginationRequest<GetFeedbacksOptions> request, PaginatedList<FeedbackDto> list, IEnumerable<UserSlimDto> users)
    {
        var skippedCount = request.GetSkipCount();

        var feedbacks = list.Items.Join(users, f => f.UserId, u => u.Id, (f, u) => (Feedback: f, User: u))
            .Select((t, i) => $"{i + skippedCount + 1}. {t.User.GetDisplayName()}: {t.Feedback.GetReportingView()}")
            .Join(Environment.NewLine);

        return Localizer["PagingInformation{Page}{PagesCount}{TotalCount}", request.Page, list.Pages, list.TotalCount]
            + Environment.NewLine
            + Environment.NewLine
            + feedbacks;
    }
}
