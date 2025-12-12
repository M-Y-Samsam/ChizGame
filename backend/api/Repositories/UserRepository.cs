using Microsoft.AspNetCore.Authorization;

namespace api.Repositories;
[Authorize]
public class UserRepository : IUserRepository
{
    #region Mongodb
    private readonly IMongoCollection<Gamer> _collection;
    private readonly ITokenService _tokenService;

    public UserRepository(IMongoClient client, IMongoDbSettings dbSettings, ITokenService tokenService)
    {
        var dbName = client.GetDatabase(dbSettings.DatabaseName);
        _collection = dbName.GetCollection<Gamer>("gamers");

        _tokenService = tokenService;
    }
    #endregion

    public async Task<LoggedInDto?> UpdateResultAsync(string userId, Gamer userInput, CancellationToken cancellationToken)
    {
        Gamer gamer = await _collection.Find<Gamer>(doc => doc.Id == userId).FirstOrDefaultAsync(cancellationToken);

        if (gamer is null) return null;

        UpdateDefinition<Gamer> updateDef = Builders<Gamer>.Update
        .Set(user => user.Email, userInput.Email.Trim().ToLower())
        .Set(user => user.Name, userInput.Name)
        .Set(user => user.Country, userInput.Country)
        .Set(user => user.City, userInput.City);

        await _collection.UpdateOneAsync(user => user.Id == userId, updateDef, null, cancellationToken);

        Gamer user = await _collection.Find<Gamer>(doc => doc.Id == userId).FirstOrDefaultAsync(cancellationToken);

        if (user is null) return null;

        string? token = _tokenService.CreateToken(user);

        return Mappers.ConvertGamerToLoggedInDto(user, token);
    }
}