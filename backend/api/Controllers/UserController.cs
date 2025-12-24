using Microsoft.AspNetCore.Authorization;

namespace api.Controllers;
public class UserController(IUserRepository userRepository) : BaseApiController
{
    [Authorize]
    [HttpPut("update/")]
    public async Task<ActionResult<MemberDto>> UpdateById(Gamer userInput , CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        if (userId is null) return Unauthorized("you are not login. Please login again");

        MemberDto? memberDto = await userRepository.UpdateResultAsync(userId,userInput, cancellationToken);

        if (memberDto is null) return BadRequest("Operation failed.");

        return Ok(memberDto);
    }    
}