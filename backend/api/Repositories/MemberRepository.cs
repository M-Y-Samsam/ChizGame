
using api.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace api.Repositories;

public class MemberRepository : IMemberRepository
{
    #region Mongodb
    private readonly IMongoCollection<Gamer> _collection;

    public MemberRepository(IMongoClient client, IMyMongoDbSettings dbSettings)
    {
        var dbName = client.GetDatabase(dbSettings.DatabaseName);
        _collection = dbName.GetCollection<Gamer>("gamers");
    }
    #endregion

    public async Task<PagedList<Gamer>> GetGamersAsync(MemberParams memberParams, CancellationToken cancellationToken)
    {
        PagedList<Gamer> gamers = await PagedList<Gamer>.CreatePagedListAsync(CreateQuery(memberParams), memberParams.PageNumber, memberParams.PageSize, cancellationToken);

        return gamers;
    }

    public async Task<MemberDto?> GetGamerByName(string userInput, CancellationToken cancellationToken)
    {
        Gamer? gamer = await _collection.Find(doc => doc.UserName == userInput).FirstOrDefaultAsync(cancellationToken);

        if (gamer is null) return null;

        MemberDto? memberDto = Mappers.ConvertGamerToMemberDto(gamer);

        return memberDto;
    }

    public IQueryable<Gamer> CreateQuery(MemberParams memberParams)
    {
        DateOnly minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-memberParams.MinAge));
        DateOnly maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-memberParams.MaxAge - 1));

        IQueryable<Gamer> query = _collection.AsQueryable();

        query = memberParams.OrderBy switch
        {
            "age" => query.OrderBy(gamer => gamer.DateOfBirth).ThenBy(gamer => gamer.Id),
            "created" => query.OrderBy(gamer => gamer.CreatedOn).ThenBy(gamer => gamer.Id),
            _ => query.OrderBy(gamer => gamer.LastActive).ThenBy(gamer => gamer.Id)
        };

        if (!string.IsNullOrEmpty(memberParams.Search))
        {
            query = query.Where(
                u => u.NormalizedUserName.Contains(memberParams.Search, StringComparison.CurrentCultureIgnoreCase)
                || u.City.Contains(memberParams.Search, StringComparison.CurrentCultureIgnoreCase)
                || u.Country.Contains(memberParams.Search, StringComparison.CurrentCultureIgnoreCase)
            );
        }

        query = query.Where(u => !(u.NormalizedUserName.Equals("ADMIN") || u.NormalizedUserName.Equals("MODERATOR")));
        query = query.Where(u => u.Id.ToString() != memberParams.UserId);
        query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);
    
        return query;
    }
}