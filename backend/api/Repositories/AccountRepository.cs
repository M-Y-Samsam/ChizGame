using Microsoft.AspNetCore.Authorization;

namespace api.Repositories
{
    [AllowAnonymous]
    public class AccountRepository : IAccountRepository
    {
        #region Mongodb
        private readonly IMongoCollection<Gamer> _collection;
        private readonly ITokenService _tokenService;

        // Dependency Injection
        public AccountRepository(IMongoClient client, IMongoDbSettings dbSettings, ITokenService tokenService)
        {
            var dbName = client.GetDatabase(dbSettings.DatabaseName);
            _collection = dbName.GetCollection<Gamer>("gamers");

            _tokenService = tokenService;
        }
        #endregion

        public async Task<LoggedInDto?> RegisterAsync(Gamer userInput, CancellationToken cancellationToken)
        {
            Gamer? user = await _collection.Find<Gamer>(doc => doc.Email == userInput.Email).FirstOrDefaultAsync(cancellationToken);

            if (user is not null)
                return null;

            await _collection.InsertOneAsync(userInput, null, cancellationToken);

            string? token = _tokenService.CreateToken(userInput);

            return Mappers.ConvertGamerToLoggedInDto(userInput, token);
        }

        public async Task<LoggedInDto?> LogInAsync(LogInDto userInput, CancellationToken cancellationToken)
        {
            Gamer? gamer = await _collection.Find(doc => doc.Email == userInput.Email && doc.Password == userInput.Password).FirstOrDefaultAsync(cancellationToken);

            if (gamer is null)
                return null;

            string? token = _tokenService.CreateToken(gamer);

            return Mappers.ConvertGamerToLoggedInDto(gamer, token);
        }

        [Authorize]
        public async Task<DeleteResult?> DeleteResultAsync(string userId, CancellationToken cancellationToken)
        {
            Gamer gamer = await _collection.Find<Gamer>(doc => doc.Id == userId).FirstOrDefaultAsync(cancellationToken);

            if (gamer is null) return null;

            return await _collection.DeleteOneAsync<Gamer>(doc => doc.Id == userId, cancellationToken);
        }

        public async Task<LoggedInDto?> ReloadLoggedInAsync(string userId, string Token, CancellationToken cancellationToken)
        {
            Gamer? gamer = await _collection.Find(doc => doc.Id == userId).FirstOrDefaultAsync(cancellationToken);

            if (gamer is null) return null;

            return Mappers.ConvertGamerToLoggedInDto(gamer, Token);
        }
    }
}