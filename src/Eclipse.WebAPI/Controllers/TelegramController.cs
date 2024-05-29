using Eclipse.Application.Contracts.Telegram;
using Eclipse.Common.Results;
using Eclipse.Pipelines.UpdateHandler;
using Eclipse.WebAPI.Filters.Authorization;
using Eclipse.WebAPI.Models;

using Microsoft.AspNetCore.Mvc;

namespace Eclipse.WebAPI.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[ApiKeyAuthorize]
public sealed class TelegramController : ControllerBase
{
    private readonly ITelegramService _service;

    private readonly IConfiguration _configuration;

    public TelegramController(ITelegramService service, IConfiguration configuration)
    {
        _service = service;
        _configuration = configuration;
    }

    [HttpPost]
    public async Task<IActionResult> Send([FromBody] SendMessageModel message, CancellationToken cancellationToken)
    {
        var result = await _service.Send(message, cancellationToken);
        return result.ToActionResult(NoContent);
    }

    [HttpPost]
    public async Task<IActionResult> SwichHandlerType([FromBody] SwichHandlerTypeRequest request, CancellationToken cancellationToken)
    {
        var telegramConfiguration = _configuration.GetSection("Telegram");

        var webhook = request.Type switch
        {
            HandlerType.Active => telegramConfiguration["WebhookUrl"],
            HandlerType.Disabled => telegramConfiguration["DisableWebhookUrl"],
            _ => string.Empty,
        };

        var result = await _service.SetWebhookUrlAsync(webhook, telegramConfiguration["SecretToken"]!, cancellationToken);

        return result.ToActionResult(Ok);
    }
}
