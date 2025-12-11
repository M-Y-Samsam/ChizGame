namespace api.Repositories;

public class UserRepository : IUserRepository
{
    #region Mongodb
    private readonly IMongoCollection<Gamer> _collection;

    public UserRepository(IMongoClient client, IMongoDbSettings dbSettings)
    {
        var dbName = client.GetDatabase(dbSettings.DatabaseName);
        _collection = dbName.GetCollection<Gamer>("gamers");
    }
    #endregion

    public async Task<LoggedInDto?> UpdateResultAsync(string userId, Gamer userInput, CancellationToken cancellationToken)
    {
        Gamer gamer = await _collection.Find<Gamer>(doc => doc.id == userId).FirstOrDefaultAsync(cancellationToken);

        if (gamer is null) return null;

        UpdateDefinition<Gamer> updateDef = Builders<Gamer>.Update
        .Set(user => user.Email, userInput.Email.Trim().ToLower())
        .Set(user => user.Name, userInput.Name)
        .Set(user => user.Country, userInput.Country)
        .Set(user => user.City, userInput.City);

        await _collection.UpdateOneAsync(user => user.id == userId, updateDef, null, cancellationToken);

        Gamer user = await _collection.Find<Gamer>(doc => doc.id == userId).FirstOrDefaultAsync(cancellationToken);

        if (user is null) return null;

        return Mappers.ConvertGamerToLoggedInDto(user);
    }
}