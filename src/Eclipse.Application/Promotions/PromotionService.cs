using Eclipse.Application.Contracts.Promotions;
using Eclipse.Common.Background;
using Eclipse.Common.Clock;
using Eclipse.Domain.Promotions;

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

    public async Task<PromotionDto> SendPromotion(SendPromotionRequest request, CancellationToken cancellationToken = default)
    {
        var promotion = Promotion.Create(request.FromChatId, request.MessageId, null, _timeProvider.Now);
        promotion.Publish();

        await _promotionsRepository.CreateAsync(promotion, cancellationToken);

        await _backgroundJobManager.EnqueueAsync<SendPromotionBackgroundJob, SendPromotionRequest>(request, cancellationToken);

        return promotion.ToDto();
    }
}
