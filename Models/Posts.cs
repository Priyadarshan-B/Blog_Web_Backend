using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;


public class Post
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] 
    public string? Id { get; set; }

    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public string? ImageUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; } = "";

    public string AuthorName { get; set; } = "";

    public int Likes { get; set; } = 0;
    public int CommentCount { get; set; } = 0;
}
