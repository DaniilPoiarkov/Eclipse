using Eclipse.Application.Contracts.Promotions;
using Eclipse.Common.Background;

namespace Eclipse.Application.Promotions;

internal sealed class PromotionService : IPromotionService
{
    private readonly IBackgroundJobManager _backgroundJobManager;

    public PromotionService(IBackgroundJobManager backgroundJobManager)
    {
        _backgroundJobManager = backgroundJobManager;
    }

    public async Task SendPromotion(SendPromotionRequest request, CancellationToken cancellationToken = default)
    {
        await _backgroundJobManager.EnqueueAsync<SendPromotionBackgroundJob, SendPromotionRequest>(request, cancellationToken);
    }
}
