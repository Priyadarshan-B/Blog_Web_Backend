using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = default!;

    public string GoogleId { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public string PictureUrl { get; set; } = default!;
}
