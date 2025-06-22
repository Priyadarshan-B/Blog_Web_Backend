using MongoDB.Driver;
using MongoDB.Bson;
using Blog_Web_Backend.DTOs;
using Microsoft.Extensions.Options;

public class FollowerService
{
    private readonly IMongoCollection<Follower> _followers;
    private readonly IMongoCollection<User> _users;

    public FollowerService(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _followers = database.GetCollection<Follower>(settings.Value.FollowerCollection);
        _users = database.GetCollection<User>(settings.Value.UserCollection);

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

    public async Task<(List<UserFollowDto> Users, int Count)> GetFollowersWithDetailsAsync(string userId)
    {
        var followers = await _followers.Find(f => f.FollowingId == userId).ToListAsync();
        var followerIds = followers.Select(f => f.FollowerId).ToList();

        var users = await _users.Find(u => followerIds.Contains(u.Id)).ToListAsync();
        var userDtos = new List<UserFollowDto>();

        foreach (var user in users)
        {
            var followersCount = await _followers.CountDocumentsAsync(f => f.FollowingId == user.Id);
            var followingCount = await _followers.CountDocumentsAsync(f => f.FollowerId == user.Id);

            userDtos.Add(new UserFollowDto
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                Email = user.Email,
                PictureUrl = user.PictureUrl,
                FollowersCount = (int)followersCount,
                FollowingCount = (int)followingCount
            });
        }

        return (userDtos, userDtos.Count);
    }

    public async Task<(List<UserFollowDto> Users, int Count)> GetFollowingWithDetailsAsync(string userId)
    {
        var following = await _followers.Find(f => f.FollowerId == userId).ToListAsync();
        var followingIds = following.Select(f => f.FollowingId).ToList();

        var users = await _users.Find(u => followingIds.Contains(u.Id)).ToListAsync();
        var userDtos = new List<UserFollowDto>();

        foreach (var user in users)
        {
            var followersCount = await _followers.CountDocumentsAsync(f => f.FollowingId == user.Id);
            var followingCount = await _followers.CountDocumentsAsync(f => f.FollowerId == user.Id);

            userDtos.Add(new UserFollowDto
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                Email = user.Email,
                PictureUrl = user.PictureUrl,
                FollowersCount = (int)followersCount,
                FollowingCount = (int)followingCount
            });
        }

        return (userDtos, userDtos.Count);
    }

    public async Task<bool> IsFollowingAsync(string followerId, string followingId)
    {
        return await _followers.Find(f =>
            f.FollowerId == followerId && f.FollowingId == followingId).AnyAsync();
    }
}
