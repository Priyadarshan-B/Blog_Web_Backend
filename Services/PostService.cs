using MongoDB.Driver;
using Microsoft.Extensions.Options;


public class PostService
{
    private readonly IMongoCollection<Post> _posts;
    private readonly IMongoCollection<Comment> _comments;

    public PostService(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _posts = database.GetCollection<Post>(settings.Value.PostCollection);
        _comments = database.GetCollection<Comment>(settings.Value.CommentCollection);
    }
    //post
    public async Task<Post> CreatePostAsync(Post post)
    {
        await _posts.InsertOneAsync(post);
        return post;
    }

    public async Task<List<Post>> GetAllPostsAsync()
    {
        return await _posts.Find(_ => true).SortByDescending(p => p.CreatedAt).ToListAsync();
    }
    //Post by id
    public async Task<Post?> GetPostByIdAsync(string id)
    {
        return await _posts.Find(p => p.Id == id).FirstOrDefaultAsync();
    }
    //comment
    public async Task<List<Comment>> GetCommentsForPostAsync(string postId) =>
    await _comments.Find(c => c.PostId == postId).SortByDescending(c => c.CreatedAt).ToListAsync();

    public async Task<Comment> AddCommentAsync(Comment comment)
    {
        await _comments.InsertOneAsync(comment);
        var update = Builders<Post>.Update.Inc(p => p.CommentCount, 1);
        await _posts.UpdateOneAsync(p => p.Id == comment.PostId, update);
        return comment;
    }
//like
    public async Task LikePostAsync(string postId)
    {
        var update = Builders<Post>.Update.Inc(p => p.Likes, 1);
        await _posts.UpdateOneAsync(p => p.Id == postId, update);
    }
}
