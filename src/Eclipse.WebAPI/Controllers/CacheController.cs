using Eclipse.Common.Caching;

using Microsoft.AspNetCore.Mvc;

namespace Eclipse.WebAPI.Controllers;

[ApiController]
[Route("api/cache")]
public sealed class CacheController : ControllerBase
{
    private readonly ICacheService _cacheService;

    public CacheController(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    [HttpDelete("prune")]
    public async Task<IActionResult> PruneAsync(CancellationToken cancellationToken)
    {
        await _cacheService.PruneAsync(cancellationToken);
        return NoContent();
    }
}
