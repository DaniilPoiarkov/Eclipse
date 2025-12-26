namespace Eclipse.Application.Contracts.Promotions;

public interface IPromotionService
{
    Task<PromotionDto> Create(CreatePromotionRequest request, CancellationToken cancellationToken = default);

    Task<PromotionDto> SendPromotion(SendPromotionRequest request, CancellationToken cancellationToken = default);
}
