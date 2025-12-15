using Eclipse.Application.Contracts.Feedbacks;
using Eclipse.Common.Caching;
using Eclipse.Core.Context;
using Eclipse.Core.Results;
using Eclipse.Core.Routing;

using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.AdminMenu.Feedbacks;

[Route("Menu:AdminMenu:Feedbacks:Request", "/admin_feedbacks_request")]
internal sealed class RequestFeedbackPipeline : AdminPipelineBase
{
    private readonly IFeedbackService _feedbackService;

    private readonly ICacheService _cacheService;

    public RequestFeedbackPipeline(IFeedbackService feedbackService, ICacheService cacheService)
    {
        _feedbackService = feedbackService;
        _cacheService = cacheService;
    }

    protected override void Initialize()
    {
        RegisterStage(SendConfirmationCode);
        RegisterStage(RequestFeedbacks);
    }

    private async Task<IResult> SendConfirmationCode(MessageContext context, CancellationToken cancellationToken)
    {
        var confirmationCode = Enumerable.Range(0, 6)
            .Select(_ => Random.Shared.Next(0, 10))
            .Aggregate(string.Empty, (s, i) => $"{s}{i}");

        await _cacheService.SetAsync(
            $"pipelines-feedbacks-request-{context.ChatId}",
            confirmationCode,
            new CacheOptions { Expiration = TimeSpan.FromMinutes(10) },
            cancellationToken
        );

        return Menu(new ReplyKeyboardRemove(), Localizer["Pipelines:Admin:Feedbacks:Request:Confirm{0}", confirmationCode]);
    }

    private async Task<IResult> RequestFeedbacks(MessageContext context, CancellationToken cancellationToken)
    {
        if (context.Value == "/cancel")
        {
            return Menu(FeedbacksButtons, Localizer["Pipelines:Admin:Feedbacks:Request:Canceled"]);
        }

        var expectedCode = await _cacheService.GetOrCreateAsync(
            $"pipelines-feedbacks-request-{context.ChatId}",
            () => Task.FromResult(string.Empty),
            cancellationToken: cancellationToken
        );

        if (context.Value != expectedCode)
        {
            return Menu(FeedbacksButtons, Localizer["Pipelines:Admin:Feedbacks:Request:Canceled"]);
        }

        await _feedbackService.RequestAsync(cancellationToken);

        return Menu(FeedbacksButtons, Localizer["Pipelines:Admin:Feedbacks:Request:Requested"]);
    }
}
