using Eclipse.Common.Linq;

namespace Eclipse.Application.Contracts.Feedbacks;

public interface IFeedbackService
{
    Task<FeedbackDto> CreateAsync(Guid userId, CreateFeedbackModel model, CancellationToken cancellationToken = default);

    Task<PaginatedList<FeedbackDto>> GetListAsync(PaginationRequest<GetFeedbacksOptions> request, CancellationToken cancellationToken = default);
}
