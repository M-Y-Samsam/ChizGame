using Microsoft.AspNetCore.Authorization;

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

    [Authorize]
    [HttpDelete("delete/{userId}")]
    public async Task<ActionResult<DeleteResult>> DeleteById(string userId, CancellationToken cancellationToken)
    {
        DeleteResult? deleteResult = await accountRepository.DeleteResultAsync(userId, cancellationToken);

        if (deleteResult is null) return BadRequest("Operation failed.");

        return Ok(deleteResult);
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<LoggedInDto>> ReloadLoggedInUser(CancellationToken cancellationToken)
    {
        string? token = null;

        bool isTokenValid = HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeader);

        if (isTokenValid)
            token = authHeader.ToString().Split(' ').Last();

        if (string.IsNullOrEmpty(token))
            return Unauthorized("Token is expired or invalid. Login again.");

        string? userId = User.GetUserId();

        if (userId is null)
            return Unauthorized();

        // get loggedInDto
        LoggedInDto? loggedInDto =
        await accountRepository.ReloadLoggedInAsync(userId, token, cancellationToken);

        return loggedInDto is null ? Unauthorized("User is logged out or unauthorized. Login again") : loggedInDto;
    }
}