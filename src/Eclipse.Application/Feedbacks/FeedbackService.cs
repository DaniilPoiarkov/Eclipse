using Eclipse.Application.Contracts.Feedbacks;
using Eclipse.Application.Feedbacks.Collection;
using Eclipse.Application.Jobs;
using Eclipse.Common.Background;
using Eclipse.Common.Clock;
using Eclipse.Common.Linq;
using Eclipse.Domain.Feedbacks;
using Eclipse.Domain.Users;

namespace Eclipse.Application.Feedbacks;

internal sealed class FeedbackService : IFeedbackService
{
    private readonly IFeedbackRepository _feedbackRepository;

    private readonly IUserRepository _userRepository;

    private readonly IBackgroundJobManager _backgroundJobManager;

    private readonly ITimeProvider _timeProvider;

    public FeedbackService(
        IFeedbackRepository feedbackRepository,
        IUserRepository userRepository,
        IBackgroundJobManager backgroundJobManager,
        ITimeProvider timeProvider)
    {
        _feedbackRepository = feedbackRepository;
        _userRepository = userRepository;
        _backgroundJobManager = backgroundJobManager;
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

    public async Task RequestAsync(CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetByExpressionAsync(u => u.IsEnabled, cancellationToken);
        
        foreach (var user in users)
        {
            await _backgroundJobManager.EnqueueAsync<RequestFeedbackBackgroundJob, UserIdJobData>(
                new UserIdJobData(user.Id),
                cancellationToken
            );
        }
    }
}
