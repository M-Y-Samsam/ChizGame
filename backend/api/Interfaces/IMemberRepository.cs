namespace api.Interfaces;

public interface IMemberRepository
{
    public Task<IEnumerable<Gamer>> GetGamersAsync(CancellationToken cancellationToken);
    public Task<MemberDto?> GetGamerByName(string userInput, CancellationToken cancellationToken);
}