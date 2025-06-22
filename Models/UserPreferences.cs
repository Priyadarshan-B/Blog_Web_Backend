using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class UserPreference
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; } = default!;

    public List<string> Preferences { get; set; } = new List<string>();
}
