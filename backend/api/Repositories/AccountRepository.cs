namespace api.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        #region Mongodb
        private readonly IMongoCollection<Gamer> _collection;

        // Dependency Injection
        public AccountRepository(IMongoClient client, IMongoDbSettings dbSettings)
        {
            var dbName = client.GetDatabase(dbSettings.DatabaseName);
            _collection = dbName.GetCollection<Gamer>("gamers");
        }
        #endregion

        public async Task<LoggedInDto?> RegisterAsync(Gamer userInput, CancellationToken cancellationToken)
        {
            Gamer? user = await _collection.Find<Gamer>(doc => doc.Email == userInput.Email).FirstOrDefaultAsync(cancellationToken);

            if (user is not null)
                return null;

            await _collection.InsertOneAsync(userInput, null, cancellationToken);

            return Mappers.ConvertGamerToLoggedInDto(userInput);
        }

        public async Task<LoggedInDto?> LogInAsync(LogInDto userInput, CancellationToken cancellationToken)
        {
            Gamer? gamer = await _collection.Find(doc => doc.Email == userInput.Email && doc.Password == userInput.Password).FirstOrDefaultAsync(cancellationToken);

            if (gamer is null)
                return null;

            return Mappers.ConvertGamerToLoggedInDto(gamer);
        }

        public async Task<DeleteResult?> DeleteResultAsync(string userId, CancellationToken cancellationToken)
        {
            Gamer gamer = await _collection.Find<Gamer>(doc => doc.id == userId).FirstOrDefaultAsync(cancellationToken);

            if (gamer is null) return null;

            return await _collection.DeleteOneAsync<Gamer>(doc => doc.id == userId, cancellationToken);
        }
    }
}