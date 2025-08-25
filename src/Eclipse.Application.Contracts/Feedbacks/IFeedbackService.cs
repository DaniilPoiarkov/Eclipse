using Eclipse.Common.Linq;

namespace Eclipse.Application.Contracts.Feedbacks;

public interface IFeedbackService
{
    Task<FeedbackDto> CreateAsync(Guid userId, CreateFeedbackModel model, CancellationToken cancellationToken = default);

    Task<PaginatedList<FeedbackDto>> GetAllAsync(int page, int size, CancellationToken cancellationToken = default);
}
