using MongoDB.Driver;
using Microsoft.Extensions.Options;


public class PostService
{
    private readonly IMongoCollection<Post> _posts;

    public PostService(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _posts = database.GetCollection<Post>(settings.Value.PostCollection);
    }

    public async Task<Post> CreatePostAsync(Post post)
    {
        await _posts.InsertOneAsync(post);
        return post;
    }

    public async Task<List<Post>> GetAllPostsAsync()
    {
        return await _posts.Find(_ => true).SortByDescending(p => p.CreatedAt).ToListAsync();
    }
}
