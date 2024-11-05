using Eclipse.Application.Contracts.Telegram;
using Eclipse.Application.Contracts.Url;
using Eclipse.Common.Results;
using Eclipse.WebAPI.Filters.Authorization;
using Eclipse.WebAPI.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Eclipse.WebAPI.Controllers;

[ApiController]
[ApiKeyAuthorize]
[Route("api/telegram")]
public sealed class TelegramController : ControllerBase
{
    private readonly ITelegramService _service;

    private readonly IConfiguration _configuration;

    private readonly IAppUrlProvider _appUrlProvider;

    private readonly IStringLocalizer<TelegramController> _stringLocalizer;

    public TelegramController(
        ITelegramService service,
        IConfiguration configuration,
        IAppUrlProvider appUrlProvider,
        IStringLocalizer<TelegramController> stringLocalizer)
    {
        _service = service;
        _configuration = configuration;
        _appUrlProvider = appUrlProvider;
        _stringLocalizer = stringLocalizer;
    }

    [HttpPost("send")]
    public async Task<IActionResult> Send([FromBody] SendMessageModel message, CancellationToken cancellationToken)
    {
        var result = await _service.Send(message, cancellationToken);
        return result.Match(NoContent, error => error.ToProblems(_stringLocalizer));
    }

    [HttpPost("switch-handler")]
    public async Task<IActionResult> SwitchHandlerType([FromBody] SwitchHandlerTypeRequest request, CancellationToken cancellationToken)
    {
        var endpoint = _configuration[$"Telegram:{Enum.GetName(request.Type)}Endpoint"];

        var result = await _service.SetWebhookUrlAsync($"{_appUrlProvider.AppUrl.EnsureEndsWith('/')}{endpoint}", cancellationToken);

        return result.Match(Ok, error => error.ToProblems(_stringLocalizer));
    }
}
