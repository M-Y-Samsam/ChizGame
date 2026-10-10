using api.Helpers;

namespace api.Interfaces;

public interface IMemberRepository
{
    public Task<PagedList<Gamer>> GetGamersAsync(MemberParams memberParams,CancellationToken cancellationToken);
    public Task<MemberDto?> GetGamerByName(string userInput, CancellationToken cancellationToken);
}