using Eclipse.Application.Contracts.Promotions;
using Eclipse.Common.Results;
using Eclipse.WebAPI.Constants;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Eclipse.WebAPI.Controllers;

[ApiController]
[Route("api/promotions")]
[Authorize(Policy = AuthorizationPolicies.Admin)]
public sealed class PromotionsController : ControllerBase
{
    private readonly IPromotionService _promotionService;

    private readonly IStringLocalizer<PromotionsController> _localizer;

    public PromotionsController(IPromotionService promotionService, IStringLocalizer<PromotionsController> localizer)
    {
        _promotionService = promotionService;
        _localizer = localizer;
    }

    [HttpGet("{id:guid}", Name = "api-promotions-find")]
    public async Task<IActionResult> Find([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var promotion = await _promotionService.Find(id, cancellationToken);
        return promotion.Match(Ok, err => err.ToProblems(_localizer));
    }

    [HttpPost("{id:guid}/publish")]
    public async Task<IActionResult> Publish([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _promotionService.Publish(id, cancellationToken);

        var url = Url.Link("api-promotions-find", id);

        return result.Match(promotion => Accepted(url, promotion), err => err.ToProblems(_localizer));
    }
}
