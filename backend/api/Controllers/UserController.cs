namespace api.Controllers;
public class UserController(IUserRepository userRepository) : BaseApiController
{
    [HttpPut("update/{userId}")]
    public async Task<ActionResult<LoggedInDto>> UpdateById(string userId , Gamer userInput , CancellationToken cancellationToken)
    {
        LoggedInDto? loggedInDto = await userRepository.UpdateResultAsync(userId,userInput, cancellationToken);

        if (loggedInDto is null) return BadRequest("Operation failed.");

        return Ok(loggedInDto);
    }    
}