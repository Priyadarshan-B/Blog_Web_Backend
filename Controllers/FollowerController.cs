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

    [HttpGet("followers/{userId}")]
    public async Task<IActionResult> GetFollowers(string userId)
    {
        var followers = await _followerService.GetFollowersAsync(userId);
        return Ok(followers);
    }

    [HttpGet("following/{userId}")]
    public async Task<IActionResult> GetFollowing(string userId)
    {
        var following = await _followerService.GetFollowingAsync(userId);
        return Ok(following);
    }

    [HttpGet("isfollowing")]
    public async Task<IActionResult> IsFollowing([FromQuery] string followerId, [FromQuery] string followingId)
    {
        var isFollowing = await _followerService.IsFollowingAsync(followerId, followingId);
        return Ok(new { isFollowing });
    }
}
