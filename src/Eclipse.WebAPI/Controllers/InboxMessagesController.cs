using Eclipse.Application.Contracts.InboxMessages;
using Eclipse.WebAPI.Constants;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eclipse.WebAPI.Controllers;

[ApiController]
[Route("api/inbox-messages")]
[Authorize(Policy = AuthorizationPolicies.Admin)]
public sealed class InboxMessagesController : ControllerBase
{
    private readonly IInboxMessageService _inboxMessageService;

    public InboxMessagesController(IInboxMessageService inboxMessageService)
    {
        _inboxMessageService = inboxMessageService;
    }

    [HttpPost("failed/reset")]
    public async Task<IActionResult> ResetFailedAsync(CancellationToken cancellationToken)
    {
        await _inboxMessageService.ResetFailedAsync(cancellationToken);
        return NoContent();
    }
}
