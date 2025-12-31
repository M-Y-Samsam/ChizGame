using api.Extensions.Validations;
using Microsoft.AspNetCore.Authorization;

namespace api.Controllers;

[Authorize]
public class UserController(IUserRepository userRepository) : BaseApiController
{
    [HttpPut("update/")]
    public async Task<ActionResult<MemberDto>> UpdateById(Gamer userInput, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        if (userId is null) return Unauthorized("you are not login. Please login again");

        MemberDto? memberDto = await userRepository.UpdateResultAsync(userId, userInput, cancellationToken);

        if (memberDto is null) return BadRequest("Operation failed.");

        return Ok(memberDto);
    }

    [HttpPut("upload-photo")]
    public async Task<ActionResult<Photo>> UploadPhoto([AllowedFileExtensions, FileSize(50_000, 4_000_000)] IFormFile file, CancellationToken cancellationToken)
    {
        if (file is null) return BadRequest("no file selcted for this request");

        string? userId = User.GetUserId();

        if (userId is null) return BadRequest("you are not logged in. please log in agaon");

        Photo? photo = await userRepository.UploadPhotoAsync(file, userId, cancellationToken);

        return photo is null ? BadRequest("Add photo failed.") : photo;
    }

    [HttpDelete("delete-photo")]
    public async Task<ActionResult<MemberDto>> DeletePhoto(CancellationToken cancellationToken)
    {
        string? userId = User.GetUserId();

        if (userId is null) return BadRequest("you are not logged in. please log in again");

        MemberDto? memberDto = await userRepository.DeleltePhotoAsync(userId, cancellationToken);

        if (memberDto is null) return BadRequest("operation failed.");

        return Ok(memberDto);
    }
}