using api.Extensions.Validations;
using Microsoft.AspNetCore.Authorization;

namespace api.Controllers;

[Authorize]
public class UserController(IUserRepository userRepository) : BaseApiController
{
    [HttpPut("update/")]
    public async Task<ActionResult<Response>> UpdateById(UserUpdateDto userInput, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        if (userId is null) return Unauthorized("you are not login. Please login again");

        UpdateResult? result = await userRepository.UpdateResultAsync(userId, userInput, cancellationToken);

        return result is null || result.ModifiedCount == 0
        ? BadRequest("Update failed, Try again later")
        : Ok(new Response( Message: "User has been updated successfully"));
    }

    [HttpPut("upload-photo")]
    public async Task<ActionResult<Response>> UploadPhoto([AllowedFileExtensions, FileSize(50_000, 4_000_000)] IFormFile file, CancellationToken cancellationToken)
    {
        if (file is null) return BadRequest("no file selcted for this request");

        string? userId = User.GetUserId();

        if (userId is null) return BadRequest("you are not logged in. please log in agaon");

        Photo? photo = await userRepository.UploadPhotoAsync(file, userId, cancellationToken);

        return photo is null 
        ? BadRequest("Add photo failed.") 
        : Ok (new Response ( Message: "photo upload successfully."));
    }

    [HttpDelete("delete-photo")]
    public async Task<ActionResult<Response>> DeletePhoto(CancellationToken cancellationToken)
    {
        string? userId = User.GetUserId();

        if (string.IsNullOrEmpty(userId)) return Unauthorized("you are not logged in. please log in again");

        UpdateResult? updateResult = await userRepository.DeleltePhotoAsync(userId, cancellationToken);

        return updateResult is null || !updateResult.IsModifiedCountAvailable
        ? BadRequest("photo deletion is failed.")
        : Ok(new Response ( Message : "photo deleted successfully." ));
    }
}