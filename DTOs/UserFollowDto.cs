namespace Blog_Web_Backend.DTOs
{
    public class UserFollowDto
    {
        public string Id { get; set; } = default!;
        public string DisplayName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string PictureUrl { get; set; } = default!;
        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }
    }
}
