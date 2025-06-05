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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPostById(string id)
    {
        var post = await _postService.GetPostByIdAsync(id);
        if (post == null)
        {
            return NotFound();
        }
        return Ok(post);
    }

    [HttpPost("{postId}/like")]
    public async Task<IActionResult> LikePost(string postId)
    {
        await _postService.LikePostAsync(postId);
        return Ok();
    }

    [HttpPost("{postId}/comments")]
    public async Task<IActionResult> AddComment(string postId, [FromBody] Comment comment)
    {
        comment.PostId = postId;
        var added = await _postService.AddCommentAsync(comment);
        return Ok(added);
    }

    [HttpGet("{postId}/comments")]
    public async Task<IActionResult> GetComments(string postId)
    {
        var comments = await _postService.GetCommentsForPostAsync(postId);
        return Ok(comments);
    }

}
