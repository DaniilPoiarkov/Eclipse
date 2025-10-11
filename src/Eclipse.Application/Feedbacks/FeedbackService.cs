using Eclipse.Application.Contracts.Feedbacks;
using Eclipse.Common.Clock;
using Eclipse.Common.Linq;
using Eclipse.Domain.Feedbacks;

namespace Eclipse.Application.Feedbacks;

internal sealed class FeedbackService : IFeedbackService
{
    private readonly IFeedbackRepository _feedbackRepository;

    private readonly ITimeProvider _timeProvider;

    public FeedbackService(IFeedbackRepository feedbackRepository, ITimeProvider timeProvider)
    {
        _feedbackRepository = feedbackRepository;
        _timeProvider = timeProvider;
    }

    public async Task<FeedbackDto> CreateAsync(Guid userId, CreateFeedbackModel model, CancellationToken cancellationToken = default)
    {
        var feedback = Feedback.Create(userId, model.Comment, model.Rate, _timeProvider.Now);

        await _feedbackRepository.CreateAsync(feedback);

        return feedback.ToDto();
    }

    public async Task<PaginatedList<FeedbackDto>> GetListAsync(PaginationRequest<GetFeedbacksOptions> request, CancellationToken cancellationToken = default)
    {
        var specification = request.Options.ToSpecification();

        var count = await _feedbackRepository.CountAsync(specification, cancellationToken);

        var feedbacks = await _feedbackRepository.GetByExpressionAsync(
            specification,
            request.GetSkipCount(),
            request.PageSize,
            cancellationToken
        );

        var models = feedbacks.Select(f => f.ToDto());

        return PaginatedList<FeedbackDto>.Create(models, count, request.PageSize);
    }
}
