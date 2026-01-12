using Eclipse.Application.Contracts.Promotions;
using Eclipse.Common.Clock;
using Eclipse.Common.Linq;
using Eclipse.Common.Results;
using Eclipse.Domain.Promotions;
using Eclipse.Domain.Shared.Errors;

namespace Eclipse.Application.Promotions;

internal sealed class PromotionService : IPromotionService
{
    private readonly IPromotionRepository _promotionsRepository;

    private readonly ITimeProvider _timeProvider;

    public PromotionService(
        IPromotionRepository promotionsRepository,
        ITimeProvider timeProvider)
    {
        _promotionsRepository = promotionsRepository;
        _timeProvider = timeProvider;
    }

    public async Task<PromotionDto> Create(CreatePromotionRequest request, CancellationToken cancellationToken = default)
    {
        var promotion = Promotion.Create(request.FromChatId, request.MessageId, request.InlineButtonText, _timeProvider.Now);

        await _promotionsRepository.CreateAsync(promotion, cancellationToken);

        return promotion.ToDto();
    }

    public async Task<Result<PromotionDto>> Find(Guid id, CancellationToken cancellationToken = default)
    {
        var promotion = await _promotionsRepository.FindAsync(id, cancellationToken);
        
        if (promotion is null)
        {
            return DefaultErrors.EntityNotFound<Promotion>();
        }

        return promotion.ToDto();
    }

    public async Task<PaginatedList<PromotionDto>> GetList(PaginationRequest<GetPromotionsOptions> request, CancellationToken cancellationToken = default)
    {
        var specification = request.Options.ToSpecification();

        var count = await _promotionsRepository.CountAsync(specification, cancellationToken);

        var promotions = await _promotionsRepository.GetByExpressionAsync(
            specification,
            request.GetSkipCount(),
            request.PageSize,
            cancellationToken
        );

        var models = promotions.Select(p => p.ToDto());

        return PaginatedList<PromotionDto>.Create(models, count, request.PageSize);
    }

    public async Task<Result<PromotionDto>> Publish(Guid id, CancellationToken cancellationToken = default)
    {
        var promotion = await _promotionsRepository.FindAsync(id, cancellationToken);

        if (promotion is null)
        {
            return DefaultErrors.EntityNotFound<Promotion>();
        }

        if (!promotion.CanRequestPublishing)
        {
            return Error.Conflict("Promotions.Publish.Request", "Cannot request publishing for promotion.");
        }

        promotion.RequestPublishing();

        await _promotionsRepository.UpdateAsync(promotion, cancellationToken);

        return promotion.ToDto();
    }
}
