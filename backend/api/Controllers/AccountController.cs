namespace api.Controllers;

public class AccountController(IAccountRepository accountRepository) : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult<LoggedInDto>> Rgister(Gamer userInput, CancellationToken cancellationToken)
    {
        if (userInput.Password != userInput.ConfirmPassword)
            return BadRequest("password and confirm Password not match");

        LoggedInDto? loggedInDto = await accountRepository.RegisterAsync(userInput, cancellationToken);

        if (loggedInDto is null)
            return BadRequest("Email is already exist!");

        return Ok(loggedInDto);
    }

    [HttpPost("Login")]
    public async Task<ActionResult<LoggedInDto>> LogIn(LogInDto userInput, CancellationToken cancellationToken)
    {
        LoggedInDto? loggedInDto = await accountRepository.LogInAsync(userInput, cancellationToken);

        if (loggedInDto is null)
            return BadRequest("Email or Password is wrong!");

        return Ok(loggedInDto);
    }

    [HttpDelete("delete/{userId}")]
    public async Task<ActionResult<DeleteResult>> DeleteById(string userId, CancellationToken cancellationToken)
    {
        DeleteResult? deleteResult = await accountRepository.DeleteResultAsync(userId, cancellationToken);

        if (deleteResult is null) return BadRequest("Operation failed.");

        return Ok(deleteResult);
    }
}