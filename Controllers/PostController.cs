using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class PostController : ControllerBase
{
    private readonly PostService _postService;

    public PostController(PostService postService)
    {
        _postService = postService;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePost([FromBody] Post post)
    {
        var created = await _postService.CreatePostAsync(post);
        return CreatedAtAction(nameof(GetAllPosts), new { id = created.Id }, created);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPosts()
    {
        var posts = await _postService.GetAllPostsAsync();
        return Ok(posts);
    }
}
