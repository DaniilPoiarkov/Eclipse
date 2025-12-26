using Eclipse.Application.Contracts.Promotions;
using Eclipse.Common.Background;
using Eclipse.Common.Clock;
using Eclipse.Common.Results;
using Eclipse.Domain.Promotions;
using Eclipse.Domain.Shared.Errors;

namespace Eclipse.Application.Promotions;

internal sealed class PromotionService : IPromotionService
{
    private readonly IPromotionRepository _promotionsRepository;

    private readonly IBackgroundJobManager _backgroundJobManager;

    private readonly ITimeProvider _timeProvider;

    public PromotionService(
        IPromotionRepository promotionsRepository,
        IBackgroundJobManager backgroundJobManager,
        ITimeProvider timeProvider)
    {
        _promotionsRepository = promotionsRepository;
        _backgroundJobManager = backgroundJobManager;
        _timeProvider = timeProvider;
    }

    public async Task<PromotionDto> Create(CreatePromotionRequest request, CancellationToken cancellationToken = default)
    {
        var promotion = Promotion.Create(request.FromChatId, request.MessageId, request.InlineButtonText, _timeProvider.Now);

        await _promotionsRepository.CreateAsync(promotion, cancellationToken);

        return promotion.ToDto();
    }

    public async Task<Result<PromotionDto>> Publish(Guid id, CancellationToken cancellationToken = default)
    {
        var promotion = await _promotionsRepository.FindAsync(id, cancellationToken);

        if (promotion is null)
        {
            return DefaultErrors.EntityNotFound<Promotion>();
        }

        promotion.Publish();

        await _promotionsRepository.UpdateAsync(promotion, cancellationToken);

        return promotion.ToDto();
    }

    public async Task<PromotionDto> SendPromotion(SendPromotionRequest request, CancellationToken cancellationToken = default)
    {
        var promotion = Promotion.Create(request.FromChatId, request.MessageId, null, _timeProvider.Now);
        promotion.Publish();

        await _promotionsRepository.CreateAsync(promotion, cancellationToken);

        await _backgroundJobManager.EnqueueAsync<SendPromotionBackgroundJob, SendPromotionRequest>(request, cancellationToken);

        return promotion.ToDto();
    }
}
