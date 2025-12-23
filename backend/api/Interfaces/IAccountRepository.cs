using api.DTOs;
using MongoDB.Driver;

namespace api.Interfaces;

public interface IAccountRepository
{
    public Task<LoggedInDto?> RegisterAsync(Gamer userInput, CancellationToken cancellationToken);
    public Task<LoggedInDto?> LogInAsync(LogInDto userInput, CancellationToken cancellationToken);
    public Task<DeleteResult?> DeleteResultAsync(string userId, CancellationToken cancellationToken);
    public Task<LoggedInDto?> ReloadLoggedInAsync(string userId, string Token, CancellationToken cancellationToken);
}