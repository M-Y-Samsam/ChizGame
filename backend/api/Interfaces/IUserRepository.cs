namespace api.Interfaces;

public interface IUserRepository
{
    public Task<LoggedInDto?> UpdateResultAsync(string userId , Gamer userInput , CancellationToken cancellationToken);
}