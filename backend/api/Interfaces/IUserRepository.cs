namespace api.Interfaces;

public interface IUserRepository
{
    public Task<MemberDto?> UpdateResultAsync(string userId , Gamer userInput , CancellationToken cancellationToken);
}