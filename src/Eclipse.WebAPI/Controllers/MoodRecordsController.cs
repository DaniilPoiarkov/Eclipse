using Eclipse.Application.Contracts.MoodRecords;

using Microsoft.AspNetCore.Mvc;

namespace Eclipse.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MoodRecordsController : ControllerBase
{
    private readonly IMoodRecordsService _moodRecordsService;

    public MoodRecordsController(IMoodRecordsService moodRecordsService)
    {
        _moodRecordsService = moodRecordsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetList()
    {
        return Ok(await _moodRecordsService.GetListAsync(Guid.NewGuid()));
    }
}
