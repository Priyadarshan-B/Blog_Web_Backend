using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Blog_Web_Backend.DTOs;

public class UserService
{
    private readonly IMongoCollection<User> _users;

    public UserService(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _users = database.GetCollection<User>(settings.Value.UserCollection);
    }

    public async Task<User> SignInWithGoogleAsync(string idToken)
    {
        var settings = new GoogleJsonWebSignature.ValidationSettings
        {
            Audience = new[] { "598155518602-f5ci4vkj1i84siubr7ad3gd4fn00sonh.apps.googleusercontent.com" }
        };

        GoogleJsonWebSignature.Payload payload;

        try
        {
            Console.WriteLine("Attempting to validate Google ID token...");
            payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            Console.WriteLine("Google ID token validated successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Google token validation failed: " + ex.Message);
            throw new InvalidOperationException("Google ID token validation failed.", ex);
        }
        if (string.IsNullOrEmpty(payload.Subject))
        {
            Console.WriteLine("ERROR: Google ID (Subject) is missing from the token payload.");
            throw new InvalidOperationException("Google ID (Subject) is missing from the token payload.");
        }
        if (string.IsNullOrEmpty(payload.Email))
        {
            Console.WriteLine("ERROR: Email is missing from the token payload.");
            throw new InvalidOperationException("Email is missing from the token payload.");
        }

        var user = await _users.Find(u => u.GoogleId == payload.Subject).FirstOrDefaultAsync();

        if (user == null)
        {
            Console.WriteLine("User not found, creating new user.");
            user = new User
            {
                GoogleId = payload.Subject,
                Email = payload.Email,
                DisplayName = payload.Name ?? payload.Email.Split('@')[0],
                PictureUrl = payload.Picture ?? string.Empty
            };
            await _users.InsertOneAsync(user);
            Console.WriteLine($"New user created with Id: {user.Id}");
        }
        else
        {
            Console.WriteLine($"Existing user found: {user.Id}");
            bool updated = false;
            if (user.DisplayName != payload.Name && payload.Name != null)
            {
                user.DisplayName = payload.Name;
                updated = true;
                Console.WriteLine("Updated DisplayName.");
            }
            if (user.PictureUrl != payload.Picture && payload.Picture != null)
            {
                user.PictureUrl = payload.Picture;
                updated = true;
                Console.WriteLine("Updated PictureUrl.");
            }
            if (user.Email != payload.Email && payload.Email != null)
            {
                user.Email = payload.Email;
                updated = true;
                Console.WriteLine("Updated Email.");
            }

            if (updated)
            {
                await _users.ReplaceOneAsync(u => u.Id == user.Id, user);
                Console.WriteLine("User document updated in MongoDB.");
            }
        }

        return user;
    }

    public async Task<User?> GetUserByIdAsync(string id) =>
        await _users.Find(u => u.Id == id).FirstOrDefaultAsync();

    public async Task<User?> GetUserByGoogleIdAsync(string googleId) =>
        await _users.Find(u => u.GoogleId == googleId).FirstOrDefaultAsync();

    public async Task<List<User>> GetAllUsersAsync() =>
        await _users.Find(_ => true).ToListAsync();

    public async Task CreateUserAsync(User user) =>
        await _users.InsertOneAsync(user);

    public async Task<bool> UpdateUserFieldsAsync(string id, UserUpdateDto updateDto)
    {
        var updates = new List<UpdateDefinition<User>>();

        if (!string.IsNullOrWhiteSpace(updateDto.Email))
            updates.Add(Builders<User>.Update.Set(u => u.Email, updateDto.Email));

        if (!string.IsNullOrWhiteSpace(updateDto.DisplayName))
            updates.Add(Builders<User>.Update.Set(u => u.DisplayName, updateDto.DisplayName));

        if (!string.IsNullOrWhiteSpace(updateDto.PictureUrl))
            updates.Add(Builders<User>.Update.Set(u => u.PictureUrl, updateDto.PictureUrl));

        if (!updates.Any())
            return false; 

        var combinedUpdate = Builders<User>.Update.Combine(updates);

        var result = await _users.UpdateOneAsync(u => u.Id == id, combinedUpdate);

        return result.ModifiedCount > 0;
    }


    public async Task<bool> UserExistsByGoogleIdAsync(string googleId) =>
        await _users.Find(u => u.GoogleId == googleId).AnyAsync();

}

