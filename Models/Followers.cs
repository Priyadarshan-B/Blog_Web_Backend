using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Follower
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = default!;

    [BsonRepresentation(BsonType.ObjectId)]
    public string FollowerId { get; set; } = default!; 

    [BsonRepresentation(BsonType.ObjectId)]
    public string FollowingId { get; set; } = default!; 
}