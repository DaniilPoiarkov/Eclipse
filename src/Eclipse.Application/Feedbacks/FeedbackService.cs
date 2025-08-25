using Eclipse.Application.Contracts.Feedbacks;
using Eclipse.Common.Linq;
using Eclipse.Domain.Feedbacks;

namespace Eclipse.Application.Feedbacks;

internal sealed class FeedbackService : IFeedbackService
{
    private readonly IFeedbackRepository _feedbackRepository;

    public FeedbackService(IFeedbackRepository feedbackRepository)
    {
        _feedbackRepository = feedbackRepository;
    }

    public async Task<FeedbackDto> CreateAsync(Guid userId, CreateFeedbackModel model, CancellationToken cancellationToken = default)
    {
        var feedback = Feedback.Create(userId, model.Comment, model.Rate);
        
        await _feedbackRepository.CreateAsync(feedback);

        return feedback.ToDto();
    }

    public Task<PaginatedList<FeedbackDto>> GetAllAsync(int page, int size, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
