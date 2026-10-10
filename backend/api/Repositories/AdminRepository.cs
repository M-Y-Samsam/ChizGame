
using Microsoft.AspNetCore.Identity;

namespace api.Repositories;

public class AdminRepository : IAdminRepository
{
    #region db & user manager setting
    private readonly IMongoCollection<Gamer>? _collection;
    private readonly UserManager<Gamer> _userManager;

    public AdminRepository(IMongoClient client, IMyMongoDbSettings dbSettings, UserManager<Gamer> userManager, ITokenService tokenService)
    {
        var database = client.GetDatabase(dbSettings.DatabaseName);
        _collection = database.GetCollection<Gamer>("users");

        _userManager = userManager;
    }
    #endregion

    public async Task<IEnumerable<UserWithRoleDto>> GetUsersWithRolesAsync()
    {
        List<UserWithRoleDto> userWitnRoleDtos = [];

        IEnumerable<Gamer> gamers = _userManager.Users;

        foreach (Gamer gamer in gamers)
        {
            IEnumerable<string> roles = await _userManager.GetRolesAsync(gamer);

            userWitnRoleDtos.Add(
                new UserWithRoleDto(
                    UserName: gamer.UserName!,
                    Roles: roles
                )
            );
        }

        return userWitnRoleDtos;

    }

    public async Task<DeleteResult?> DeletUserAsync(string targetUserName, CancellationToken cancellationToken)
    {
        Gamer gamer = await _collection.Find(user => user.NormalizedUserName == targetUserName.ToUpper()).FirstOrDefaultAsync();

        if (gamer is null) return null;

        return await _collection.DeleteOneAsync(doc => doc.Id == gamer.Id, cancellationToken);
    }
}