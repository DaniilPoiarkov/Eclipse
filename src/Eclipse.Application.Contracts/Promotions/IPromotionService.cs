using Eclipse.Common.Linq;
using Eclipse.Common.Results;

namespace Eclipse.Application.Contracts.Promotions;

public interface IPromotionService
{
    Task<Result<PromotionDto>> Find(Guid id, CancellationToken cancellationToken = default);

    Task<PromotionDto> Create(CreatePromotionRequest request, CancellationToken cancellationToken = default);

    Task<Result<PromotionDto>> Publish(Guid id, CancellationToken cancellationToken = default);

    Task<PaginatedList<PromotionDto>> GetList(PaginationRequest<GetPromotionsOptions> request, CancellationToken cancellationToken = default);
}
