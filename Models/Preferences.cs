using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Preference
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("Category")]
    public string Category { get; set; } = default!;

    [BsonElement("Topic")]
    public string Topic { get; set; } = default!;
}
