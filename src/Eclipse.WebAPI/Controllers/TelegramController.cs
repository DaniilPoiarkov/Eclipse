using Eclipse.Application.Contracts.Telegram;
using Eclipse.Application.Contracts.Url;
using Eclipse.WebAPI.Filters.Authorization;
using Eclipse.WebAPI.Models;

using Microsoft.AspNetCore.Mvc;

namespace Eclipse.WebAPI.Controllers;

[ApiController]
[ApiKeyAuthorize]
[Route("api/telegram")]
public sealed class TelegramController : ControllerBase
{
    private readonly ITelegramService _service;

    private readonly IConfiguration _configuration;

    private readonly IAppUrlProvider _appUrlProvider;

    public TelegramController(ITelegramService service, IConfiguration configuration, IAppUrlProvider appUrlProvider)
    {
        _service = service;
        _configuration = configuration;
        _appUrlProvider = appUrlProvider;
    }

    [HttpPost("send")]
    public async Task<IActionResult> Send([FromBody] SendMessageModel message, CancellationToken cancellationToken)
    {
        var result = await _service.Send(message, cancellationToken);
        return result.ToActionResult(NoContent);
    }

    [HttpPost("switch-handler")]
    public async Task<IActionResult> SwichHandlerType([FromBody] SwichHandlerTypeRequest request, CancellationToken cancellationToken)
    {
        var endpoint = _configuration[$"Telegram:{Enum.GetName(request.Type)}Endpoint"];

        var result = await _service.SetWebhookUrlAsync($"{_appUrlProvider.AppUrl}/{endpoint}", cancellationToken);

        return result.ToActionResult(Ok);
    }
}
