using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/")]
public class PreferenceController : ControllerBase
{
    private readonly PreferenceService _preferenceService;

    public PreferenceController(PreferenceService preferenceService)
    {
        _preferenceService = preferenceService;
    }

    [HttpPost("preferences")]
    public async Task<IActionResult> CreatePreference([FromBody] Preference preference)
    {
        if (string.IsNullOrWhiteSpace(preference.Category))
            return BadRequest("Category is required.");

        if (string.IsNullOrWhiteSpace(preference.Topic))
            return BadRequest("Topic name is required.");

        var existing = await _preferenceService.GetByCategoryAndTopicAsync(preference.Category, preference.Topic);
        if (existing != null)
            return Conflict("Topic already exists in the category.");

        var createdPreference = await _preferenceService.CreatePreferenceAsync(preference);
        return CreatedAtAction(nameof(GetAllPreferences), new { id = createdPreference.Id }, createdPreference);
    }
    [HttpGet("preferences")]
    public async Task<IActionResult> GetAllPreferences()
    {
        var preferences = await _preferenceService.GetAllPreferencesAsync();
        return Ok(preferences);
    }

    [HttpGet("preferences/grouped")]
    public async Task<IActionResult> GetGroupedPreferences()
    {
        var allPreferences = await _preferenceService.GetAllPreferencesAsync();

        var grouped = allPreferences
            .GroupBy(p => p.Category)
            .ToDictionary(g => g.Key, g => g.Select(p => p.Topic).ToList());

        return Ok(grouped);
    }

    [HttpPost("preferences/bulk")]
    public async Task<IActionResult> CreatePreferencesBulk([FromBody] Dictionary<string, List<string>> categoryTopics)
    {
        var newPreferences = new List<Preference>();

        foreach (var category in categoryTopics.Keys)
        {
            foreach (var topic in categoryTopics[category])
            {
                if (string.IsNullOrWhiteSpace(topic) || string.IsNullOrWhiteSpace(category))
                    continue;

                var exists = await _preferenceService.GetByCategoryAndTopicAsync(category, topic);
                if (exists == null)
                {
                    newPreferences.Add(new Preference { Category = category, Topic = topic });
                }
            }
        }

        if (newPreferences.Count == 0)
            return Conflict("No new topics to add.");

        await _preferenceService.CreatePreferencesBulkAsync(newPreferences);

        return Created("api/preferences/bulk", newPreferences);
    }

}
