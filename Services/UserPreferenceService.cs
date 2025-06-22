using MongoDB.Driver;
using Microsoft.Extensions.Options;

public class UserPreferenceService
{
    private readonly IMongoCollection<UserPreference> _userPreferences;
    public UserPreferenceService(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _userPreferences = database.GetCollection<UserPreference>(settings.Value.UserPreferenceCollection);
         
    }

    public async Task<UserPreference?> GetByUserIdAsync(string userId)
    {
        return await _userPreferences.Find(up => up.UserId == userId).FirstOrDefaultAsync();
    }

    public async Task<UserPreference> AddOrUpdatePreferencesAsync(string userId, List<string> newPreferences)
    {
        var existing = await GetByUserIdAsync(userId);

        if (existing == null)
        {
            var newPref = new UserPreference
            {
                UserId = userId,
                Preferences = newPreferences.Distinct().ToList()
            };
            await _userPreferences.InsertOneAsync(newPref);
            return newPref;
        }

        var merged = existing.Preferences.Union(newPreferences).Distinct().ToList();

        var update = Builders<UserPreference>.Update.Set(up => up.Preferences, merged);
        await _userPreferences.UpdateOneAsync(up => up.UserId == userId, update);

        existing.Preferences = merged;
        return existing;
    }
}