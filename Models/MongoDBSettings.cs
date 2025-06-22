public class MongoDbSettings
{
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
    public string PostCollection { get; set; } = default!;
    public string UserCollection { get; set; } = default!;
    public string CommentCollection { get; set; } = default!;
    public string FollowerCollection { get; set; } = default!;
    public string PreferenceCollection { get; set; } = "Preferences";
    public string UserPreferenceCollection { get; set; } = "UserPreferences";
}