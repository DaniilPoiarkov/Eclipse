using Eclipse.Common.Telegram;
using Eclipse.Domain.Users;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Eclipse.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AAATestController : ControllerBase
{
    private readonly IOptions<TelegramOptions> _options;

    private readonly UserManager _userManager;

    public AAATestController(IOptions<TelegramOptions> options, UserManager userManager)
    {
        _options = options;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Test()
    {
        var user = await _userManager.FindByChatIdAsync(_options.Value.Chat);

        if (user is null)
        {
            return NotFound();
        }

        user.TriggerTestEvent();

        await _userManager.UpdateAsync(user);

        return Ok();
    }
}
