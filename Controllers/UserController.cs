using Microsoft.AspNetCore.Mvc;
using Blog_Web_Backend.DTOs;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;
    private readonly PostService _postService;

    public UserController(UserService userService, PostService postService)
    {
        _userService = userService;
        _postService = postService; 
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(string id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] User user)
    {
        await _userService.CreateUserAsync(user);
        return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
    }

    [HttpPatch("{id}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdateUser(string id, [FromForm] UserUpdateDto updateDto, [FromForm] IFormFile? imageFile)
    {
        var existingUser = await _userService.GetUserByIdAsync(id);
        if (existingUser == null)
            return NotFound();

        if (imageFile != null && imageFile.Length > 0)
        {
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
            using var stream = imageFile.OpenReadStream();
            var imageUrl = await _postService.UploadImageToSupabaseAsync(stream, fileName, imageFile.ContentType);
            updateDto.PictureUrl = imageUrl;
        }

        var success = await _userService.UpdateUserFieldsAsync(id, updateDto);
        if (!success)
            return BadRequest("No valid fields provided for update.");

        return Ok(await _userService.GetUserByIdAsync(id));
    }


}

