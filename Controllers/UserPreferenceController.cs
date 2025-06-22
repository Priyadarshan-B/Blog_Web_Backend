using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UserPreferenceController : ControllerBase
{
    private readonly UserPreferenceService _service;

    public UserPreferenceController(UserPreferenceService service)
    {
        _service = service;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> Get(string userId)
    {
        var pref = await _service.GetByUserIdAsync(userId);

        if (pref == null)
            return NotFound();
        pref.Preferences ??= new List<string>();

        return Ok(pref);
    }

    [HttpPost("{userId}")]
    public async Task<IActionResult> Post(string userId, [FromBody] List<string> preferences)
    {
        if (preferences == null || preferences.Count == 0)
            return BadRequest("Preferences cannot be empty.");

        var result = await _service.AddOrUpdatePreferencesAsync(userId, preferences);
        return Ok(result);
    }
}
