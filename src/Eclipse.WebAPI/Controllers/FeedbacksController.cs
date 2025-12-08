using Eclipse.Application.Contracts.Feedbacks;
using Eclipse.Common.Linq;
using Eclipse.WebAPI.Constants;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eclipse.WebAPI.Controllers;

[ApiController]
[Route("api/feedbacks")]
[Authorize(Policy = AuthorizationPolicies.Admin)]
public sealed class FeedbacksController : ControllerBase
{
    private readonly IFeedbackService _feedbackService;

    public FeedbacksController(IFeedbackService feedbackService)
    {
        _feedbackService = feedbackService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync([FromQuery] PaginationRequest<GetFeedbacksOptions> request, CancellationToken cancellationToken = default)
    {
        var feedbacks = await _feedbackService.GetListAsync(request, cancellationToken);
        return Ok(feedbacks);
    }

    [HttpPut("request")]
    public async Task<IActionResult> RequestAsync(CancellationToken cancellationToken = default)
    {
        await _feedbackService.RequestAsync(cancellationToken);
        return Accepted();
    }
}
