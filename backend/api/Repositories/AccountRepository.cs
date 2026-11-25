using AspNetCore.Identity.MongoDbCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace api.Repositories
{
    [AllowAnonymous]
    public class AccountRepository : IAccountRepository
    {
        #region Mongodb
        private readonly IMongoCollection<Gamer> _collection;
        private readonly ITokenService _tokenService;
        private readonly UserManager<Gamer> _userManager;
        // Dependency Injection
        public AccountRepository(IMongoClient client, IMyMongoDbSettings dbSettings, ITokenService tokenService, UserManager<Gamer> userManager)
        {
            var dbName = client.GetDatabase(dbSettings.DatabaseName);
            _collection = dbName.GetCollection<Gamer>("gamers");
            _userManager = userManager;
            _tokenService = tokenService;
        }
        #endregion

        public async Task<LoggedInDto?> RegisterAsync(RegisterDto userInput, CancellationToken cancellationToken)
        {
            Gamer gamer = Mappers.ConvertRegisterDtoToGamer(userInput);

            IdentityResult userCreationResult = await _userManager.CreateAsync(gamer, userInput.Password);

            if (!userCreationResult.Succeeded)
            {
                var errors = userCreationResult.Errors
                .Select(e => e.Description)
                .ToList();

                return new LoggedInDto
                {
                    Errors = errors
                };
            }

            IdentityResult roleReslt = await _userManager.AddToRoleAsync(gamer, "gamer");

            if (!roleReslt.Succeeded)
            {
                var roleErrors = roleReslt.Errors
                .Select(e => e.Description)
                .ToList();

                return new LoggedInDto
                {
                    Errors = roleErrors
                };
            }

            string? token = await _tokenService.CreateTokenAsync(gamer, cancellationToken);

            if (string.IsNullOrEmpty(token))
            {
                return new LoggedInDto
                {
                    Errors = new List<string> { "Fieled to gnerate authentication token." }
                };
            }

            return Mappers.ConvertGamerToLoggedInDto(gamer, token);
        }

        public async Task<LoggedInDto?> LoginAsync(LogInDto userInput, CancellationToken cancellationToken)
        {
            Gamer? gamer = await _userManager.FindByNameAsync(userInput.Name);

            if (gamer is null)
            {
                return new LoggedInDto
                {
                    IsWrongCred = true
                };
            }

            bool isPassCorrect = await _userManager.CheckPasswordAsync(gamer, userInput.Password);

            if (!isPassCorrect)
            {
                return new LoggedInDto
                {
                    IsWrongCred = true
                };
            }

            string? token = await _tokenService.CreateTokenAsync(gamer, cancellationToken);

            if (string.IsNullOrEmpty(token))
                return new LoggedInDto
                {
                    Errors = new List<string> { "Fieled to gnerate authentication token." }
                };

            return Mappers.ConvertGamerToLoggedInDto(gamer, token);
        }

        [Authorize]
        public async Task<DeleteResult?> DeleteResultAsync(string userIdHashed, CancellationToken cancellationToken)
        {
            ObjectId? userId = await _tokenService.GetActualUserId(userIdHashed, cancellationToken);

            Gamer gamer = await _collection.Find<Gamer>(doc => doc.Id == userId).FirstOrDefaultAsync(cancellationToken);

            if (gamer is null) return null;

            return await _collection.DeleteOneAsync<Gamer>(doc => doc.Id == userId, cancellationToken);
        }

        public async Task<LoggedInDto?> ReloadLoggedInAsync(string userIdHashed, string Token, CancellationToken cancellationToken)
        {
            ObjectId? userId = await _tokenService.GetActualUserId(userIdHashed, cancellationToken);

            Gamer? gamer = await _collection.Find(doc => doc.Id == userId).FirstOrDefaultAsync(cancellationToken);

            if (gamer is null) return null;

            return Mappers.ConvertGamerToLoggedInDto(gamer, Token);
        }

        public async Task<UpdateResult?> UpdateLastActive(string userIdHashed, CancellationToken cancellationToken)
        {
            if (userIdHashed is null) return null;

            UpdateDefinition<Gamer> updateLastActive = Builders<Gamer>.Update
            .Set(user => user.LastActive, DateTime.UtcNow);

            ObjectId? userId = await _tokenService.GetActualUserId(userIdHashed, cancellationToken);

            return await _collection.UpdateOneAsync(doc => doc.Id == userId, updateLastActive, null, cancellationToken);
        }
    }
}