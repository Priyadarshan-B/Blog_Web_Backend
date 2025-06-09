using MongoDB.Driver;
using Microsoft.Extensions.Options;

public class PreferenceService
{
    private readonly IMongoCollection<Preference> _preferences;

    public PreferenceService(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _preferences = database.GetCollection<Preference>(settings.Value.PreferenceCollection);
    }

    public async Task<Preference> CreatePreferenceAsync(Preference preference)
    {
        await _preferences.InsertOneAsync(preference);
        return preference;
    }

    public async Task<List<Preference>> GetAllPreferencesAsync()
    {
        return await _preferences.Find(_ => true).ToListAsync();
    }

    public async Task<Preference?> GetByCategoryAndTopicAsync(string category, string topic)
    {
        return await _preferences.Find(p =>
            p.Category.ToLower() == category.ToLower() &&
            p.Topic.ToLower() == topic.ToLower())
            .FirstOrDefaultAsync();
    }

    public async Task CreatePreferencesBulkAsync(List<Preference> preferences)
    {
        await _preferences.InsertManyAsync(preferences);
    }

}
