namespace Eclipse.Application.Contracts.Promotions;

public interface IPromotionService
{
    Task<PromotionDto> SendPromotion(SendPromotionRequest request, CancellationToken cancellationToken = default);
}
