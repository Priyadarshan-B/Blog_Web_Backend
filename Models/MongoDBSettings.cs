public class MongoDbSettings
{
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
    public string PostCollection { get; set; } = default!;
    public string UserCollection { get; set; } = default!;
}