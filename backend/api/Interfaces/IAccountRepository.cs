using api.DTOs;
using MongoDB.Driver;

namespace api.Interfaces;

public interface IAccountRepository
{
    public Task<LoggedInDto?> RegisterAsync(RegisterDto userInput, CancellationToken cancellationToken);
    public Task<LoggedInDto?> LoginAsync(LogInDto userInput, CancellationToken cancellationToken);
    public Task<DeleteResult?> DeleteResultAsync(string userIdHashed, CancellationToken cancellationToken);
    public Task<LoggedInDto?> ReloadLoggedInAsync(string userIdHashed, string Token, CancellationToken cancellationToken);
    public Task<UpdateResult?> UpdateLastActive(string userIdHashed, CancellationToken cancellationToken);
}