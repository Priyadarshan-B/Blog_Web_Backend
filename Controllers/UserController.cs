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

    [HttpPatch("{id}/image")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdateUserImage(string id, [FromForm] UserUpdateFormDto formDto)
    {
        var existingUser = await _userService.GetUserByIdAsync(id);
        if (existingUser == null)
            return NotFound();

        if (formDto.ImageFile == null || formDto.ImageFile.Length == 0)
            return BadRequest("No image file uploaded.");

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(formDto.ImageFile.FileName)}";
        using var stream = formDto.ImageFile.OpenReadStream();
        var imageUrl = await _postService.UploadImageToSupabaseAsync(stream, fileName, formDto.ImageFile.ContentType, "blog-web-users");

        var updateDto = new UserUpdateDto
        {
            PictureUrl = imageUrl
        };

        var success = await _userService.UpdateUserFieldsAsync(id, updateDto);
        if (!success)
            return BadRequest("Image update failed.");

        return Ok(await _userService.GetUserByIdAsync(id));
    }


    [HttpPatch("{id}/basic")]
    [Consumes("application/json")]
    public async Task<IActionResult> UpdateUserBasic(string id, [FromBody] UserBasicDto updateDto)
    {
        var existingUser = await _userService.GetUserByIdAsync(id);
        if (existingUser == null)
            return NotFound();

        var userUpdateDto = new UserUpdateDto
        {
            Email = updateDto.Email,
            DisplayName = updateDto.DisplayName
        };

        var success = await _userService.UpdateUserFieldsAsync(id, userUpdateDto);
        if (!success)
            return BadRequest("No valid fields provided for update.");

        return Ok(await _userService.GetUserByIdAsync(id));
    }



}

