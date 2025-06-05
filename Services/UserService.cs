using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

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
        var payload = await GoogleJsonWebSignature.ValidateAsync(idToken);

        var user = await _users.Find(u => u.GoogleId == payload.Subject).FirstOrDefaultAsync();

        if (user == null)
        {
            
            user = new User
            {
                GoogleId = payload.Subject,
                Email = payload.Email,
                DisplayName = payload.Name
            };
            await _users.InsertOneAsync(user);
        }

        return user;
    }
}
