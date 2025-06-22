using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/follower")]
public class FollowerController : ControllerBase
{
    private readonly FollowerService _followerService;

    public FollowerController(FollowerService followerService)
    {
        _followerService = followerService;
    }

    [HttpPost("follow")]
    public async Task<IActionResult> Follow([FromQuery] string followerId, [FromQuery] string followingId)
    {
        await _followerService.FollowAsync(followerId, followingId);
        return Ok(new { success = true });
    }

    [HttpPost("unfollow")]
    public async Task<IActionResult> Unfollow([FromQuery] string followerId, [FromQuery] string followingId)
    {
        await _followerService.UnfollowAsync(followerId, followingId);
        return Ok(new { success = true });
    }

    [HttpGet("{userId}/followers")]
    public async Task<IActionResult> GetFollowers(string userId)
    {
        var (followers, count) = await _followerService.GetFollowersWithDetailsAsync(userId);
        return Ok(new { count, followers });
    }

    [HttpGet("{userId}/following")]
    public async Task<IActionResult> GetFollowing(string userId)
    {
        var (following, count) = await _followerService.GetFollowingWithDetailsAsync(userId);
        return Ok(new { count, following });
    }

    [HttpGet("isfollowing")]
    public async Task<IActionResult> IsFollowing([FromQuery] string followerId, [FromQuery] string followingId)
    {
        var isFollowing = await _followerService.IsFollowingAsync(followerId, followingId);
        return Ok(new { isFollowing });
    }
}
