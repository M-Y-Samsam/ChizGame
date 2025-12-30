
using Microsoft.AspNetCore.Authorization;

namespace api.Repositories;

public class MemberRepository : IMemberRepository
{
    #region Mongodb
    private readonly IMongoCollection<Gamer> _collection;

    public MemberRepository(IMongoClient client, IMongoDbSettings dbSettings)
    {
        var dbName = client.GetDatabase(dbSettings.DatabaseName);
        _collection = dbName.GetCollection<Gamer>("gamers");
    }
    #endregion

    public async Task<IEnumerable<Gamer>?> GetGamersAsync(CancellationToken cancellationToken)
    {
        IEnumerable<Gamer>? gamers = await _collection.Find<Gamer>(new BsonDocument()).ToListAsync(cancellationToken);

        if (gamers.Any())
            return null;

        return gamers;
    }

    public async Task<MemberDto?> GetGamerByName(string userInput, CancellationToken cancellationToken)
    {
        Gamer? gamer = await _collection.Find(doc => doc.Name == userInput).FirstOrDefaultAsync(cancellationToken);

        if (gamer is null) return null;

        MemberDto? memberDto = Mappers.ConvertGamerToMemberDto(gamer);

        return memberDto;
    }
}