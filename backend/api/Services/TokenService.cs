
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver.Linq;

namespace api.Services;

public class TokenService : ITokenService
{
    private readonly IMongoCollection<Gamer> _collection;
    private readonly SymmetricSecurityKey? _key;
    private readonly UserManager<Gamer> _userManager;

    public TokenService(IConfiguration config, IMongoClient client, IMyMongoDbSettings dbSettings, UserManager<Gamer> userManager)
    {
        var database = client.GetDatabase(dbSettings.DatabaseName);
        _collection = database.GetCollection<Gamer>("gamers");
        // _collection = database.GetCollection<AppUser>(AppVariablesExtensions.CollectionUsers);

        string? tokenValue = config.GetValue<string>("TokenKey");
        // string? tokenValue = config.GetValue<string>(AppVariablesExtensions.TokenKey);

        _ = tokenValue ?? throw new ArgumentException("token key cannot be null", nameof(tokenValue));

        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenValue!));

        _userManager = userManager;
    }

    public async Task<string?> CreateTokenAsync(Gamer gamer, CancellationToken cancellationToken)
    {
        _ = _key ?? throw new ArgumentException("_key cannot be null", nameof(_key));

        string? userIdeHashed = await InsertEncryptedUserId(gamer.Id, cancellationToken);

        if (string.IsNullOrEmpty(userIdeHashed)) return null;

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.NameId, userIdeHashed),
            // new Claim(JwtRegisteredClaimNames.Name, gamer.Name!),
        };

        //get user`s roles and add them all into claims
        IList<string>? roles = await _userManager.GetRolesAsync(gamer);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(7),
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public async Task<ObjectId?> GetActualUserId(string? userIdHashed, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(userIdHashed)) return null;

        ObjectId? userId = await _collection.AsQueryable()
        .Where(gamer => gamer.IdentifireHash == userIdHashed)
        .Select(gamer => gamer.Id)
        .SingleOrDefaultAsync(cancellationToken);

        return (userId is null || !userId.HasValue)
        ? null
        : userId;
    }

    private async Task<string?> InsertEncryptedUserId(ObjectId userId, CancellationToken cancellationToken, string? jtiValue = null)
    {
        string newObjectId = ObjectId.GenerateNewId().ToString();

        string identifireHash = BCrypt.Net.BCrypt.HashPassword(newObjectId);

        UpdateDefinition<Gamer> updatedSecuredToken = Builders<Gamer>.Update
        .Set(gamer => gamer.IdentifireHash, identifireHash);
        // .Set(appUser => appUser.JtiValue, jtiValue);

        UpdateResult updateResult = await _collection.UpdateOneAsync<Gamer>(gamer => gamer.Id == userId, updatedSecuredToken, null, cancellationToken);

        if (updateResult.ModifiedCount == 1) return identifireHash;

        return null;
    }
}