using MongoDB.Driver;
using Microsoft.Extensions.Options;
using Supabase;
using System.IO;
using Microsoft.Extensions.Configuration; 

public class PostService
{
    private readonly IMongoCollection<Post> _posts;
    private readonly IMongoCollection<Comment> _comments;
    private readonly Supabase.Client _supabaseClient;
    private readonly IConfiguration _configuration; 

    public PostService(IOptions<MongoDbSettings> settings, IConfiguration configuration)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _posts = database.GetCollection<Post>(settings.Value.PostCollection);
        _comments = database.GetCollection<Comment>(settings.Value.CommentCollection);

        // Initialize _configuration here
        _configuration = configuration;

        var supabaseUrl = _configuration["Supabase:Url"];
        var supabaseKey = _configuration["Supabase:Key"];
        if (string.IsNullOrEmpty(supabaseUrl) || string.IsNullOrEmpty(supabaseKey))
        {
            throw new ArgumentNullException("Supabase:Url or Supabase:Key is not configured in appsettings.json.");
        }
        _supabaseClient = new Supabase.Client(supabaseUrl, supabaseKey);
    }

    // Post operations
    public async Task<Post> CreatePostAsync(Post post)
    {
        await _posts.InsertOneAsync(post);
        return post;
    }

    public async Task<List<Post>> GetAllPostsAsync()
    {
        return await _posts.Find(_ => true).SortByDescending(p => p.CreatedAt).ToListAsync();
    }

    public async Task<List<Post>> GetPostsByPreferenceAsync(string preference)
    {
        var filter = Builders<Post>.Filter.AnyEq(p => p.Preferences, preference);
        return await _posts.Find(filter).SortByDescending(p => p.CreatedAt).ToListAsync();
    }

    // Post by id
    public async Task<Post?> GetPostByIdAsync(string id)
    {
        return await _posts.Find(p => p.Id == id).FirstOrDefaultAsync();
    }

    // Comment operations
    public async Task<List<Comment>> GetCommentsForPostAsync(string postId) =>
        await _comments.Find(c => c.PostId == postId).SortByDescending(c => c.CreatedAt).ToListAsync();

    public async Task<Comment> AddCommentAsync(Comment comment)
    {
        await _comments.InsertOneAsync(comment);
        var update = Builders<Post>.Update.Inc(p => p.CommentCount, 1);
        await _posts.UpdateOneAsync(p => p.Id == comment.PostId, update);
        return comment;
    }

    // Like operation
    public async Task LikePostAsync(string postId)
    {
        var update = Builders<Post>.Update.Inc(p => p.Likes, 1);
        await _posts.UpdateOneAsync(p => p.Id == postId, update);
    }

    public async Task<string> UploadImageToSupabaseAsync(Stream fileStream, string fileName, string contentType)
    {
       
        await _supabaseClient.InitializeAsync(); 

        var bucket = _supabaseClient.Storage.From("blog-web");

        using var memoryStream = new MemoryStream();
        await fileStream.CopyToAsync(memoryStream);
        byte[] fileBytes = memoryStream.ToArray();

        await bucket.Upload(fileBytes, fileName, new Supabase.Storage.FileOptions
        {
            Upsert = true,
            ContentType = contentType
        });

        return bucket.GetPublicUrl(fileName);
    }
}
