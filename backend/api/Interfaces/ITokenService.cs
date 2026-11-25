namespace api.Interfaces;

public interface ITokenService
{
    public Task<string?> CreateTokenAsync(Gamer gamer, CancellationToken cancellationToken);
    public Task<ObjectId?> GetActualUserId(string? userIdHashed, CancellationToken cancellationToken);
}