namespace Eclipse.Application.Contracts.Promotions;

public interface IPromotionService
{
    Task SendPromotion(SendPromotionRequest request, CancellationToken cancellationToken = default);
}
