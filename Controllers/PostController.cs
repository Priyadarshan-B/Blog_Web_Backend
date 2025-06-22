using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.IO;
//using Blog_Web_Backend.Models;

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
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> CreatePost([FromForm] Post post, IFormFile? imageFile)
    {
        if (imageFile != null && imageFile.Length > 0)
        {
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";

            using var stream = imageFile.OpenReadStream();
            var imageUrl = await _postService.UploadImageToSupabaseAsync(stream, fileName, imageFile.ContentType);
            post.ImageUrl = imageUrl;
        }

        post.CreatedAt = DateTime.UtcNow;

        var created = await _postService.CreatePostAsync(post);
        return CreatedAtAction(nameof(GetPostById), new { id = created.Id }, created);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPosts()
    {
        var posts = await _postService.GetAllPostsAsync();
        return Ok(posts);
    }

    [HttpGet("preference/{topic}")]
    public async Task<IActionResult> GetPostsByPreference(string topic)
    {
        var posts = await _postService.GetPostsByPreferenceAsync(topic);
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

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetPostsByUserId(string userId)
    {
        var posts = await _postService.GetPostsByUserIdAsync(userId);
        return Ok(posts);
    }


}
