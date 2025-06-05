
using MongoDB.Driver;
using Microsoft.Extensions.Options;

public class FollowerService
{
    private readonly IMongoCollection<Follower> _followers;

    public FollowerService(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _followers = database.GetCollection<Follower>(settings.Value.FollowerCollection);
    }

    public async Task FollowAsync(string followerId, string followingId)
    {
        var exists = await _followers.Find(f =>
            f.FollowerId == followerId && f.FollowingId == followingId).AnyAsync();

        if (!exists)
        {
            await _followers.InsertOneAsync(new Follower
            {
                FollowerId = followerId,
                FollowingId = followingId
            });
        }
    }

    public async Task UnfollowAsync(string followerId, string followingId)
    {
        await _followers.DeleteOneAsync(f =>
            f.FollowerId == followerId && f.FollowingId == followingId);
    }

    public async Task<List<Follower>> GetFollowersAsync(string userId)
    {
        return await _followers.Find(f => f.FollowingId == userId).ToListAsync();
    }

    public async Task<List<Follower>> GetFollowingAsync(string userId)
    {
        return await _followers.Find(f => f.FollowerId == userId).ToListAsync();
    }

    public async Task<bool> IsFollowingAsync(string followerId, string followingId)
    {
        return await _followers.Find(f =>
            f.FollowerId == followerId && f.FollowingId == followingId).AnyAsync();
    }
}
