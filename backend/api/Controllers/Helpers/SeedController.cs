using Microsoft.AspNetCore.Identity;

namespace api.Controllers.Helpers;

public class SeedController : BaseApiController
{
    #region Db
    private readonly IMongoDatabase _database;
    private readonly IMongoClient _client;
    private readonly UserManager<Gamer> _userManager;
    private readonly RoleManager<AppRole> _roleManager;

    public SeedController(IMongoClient client, IMyMongoDbSettings dbSettings, UserManager<Gamer> userManager, RoleManager<AppRole> roleManager)
    {
        _client = client;
        _database = client.GetDatabase(dbSettings.DatabaseName);

        _userManager = userManager;
        _roleManager = roleManager;
    }
    #endregion

    [HttpPost]
    public async Task<ActionResult> CreateDummyMembers()
    {
        #region If databaseExists
        // Stop if database is already exist using its status
        // https://stackoverflow.com/a/53803908/3944285
        var command = "{ dbStats: 1, scale:1 }";
        var dbStats = await _database.RunCommandAsync<BsonDocument>(command);
        bool databaseExist;

        if (dbStats["collections"].BsonType == BsonType.Int64)
        {
            var collectionsCount = dbStats["collections"].AsInt64;
            databaseExist = collectionsCount > 0 || dbStats["indexes"].AsInt64 > 0;
        }

        else
        {
            var collectionCount = dbStats["collection"].AsInt32;
            databaseExist = collectionCount > 0 || dbStats["indexes"].AsInt32 > 0;
        }

        if (databaseExist == true)
            await _client.DropDatabaseAsync("ChizGame");
        #endregion

        #region create role
        await _roleManager.CreateAsync(new AppRole { Name = "admin" });
        await _roleManager.CreateAsync(new AppRole { Name = "moderator" });
        await _roleManager.CreateAsync(new AppRole { Name = "gamer" });
        #endregion

        #region create admin & moderator
        //Admin 
        Gamer admin = new()
        {
            UserName = "admin",
            Email = "admin@a.com"
        };

        await _userManager.CreateAsync(admin, "Aaaaaaaa/");
        await _userManager.AddToRolesAsync(admin, ["admin", "moderator"]);

        //moderator
        Gamer moderator = new()
        {
            UserName = "moderator",
            Email = "moderator@a.com"
        };

        await _userManager.CreateAsync(moderator, "Aaaaaaaa/");
        await _userManager.AddToRoleAsync(moderator, "moderator");
        #endregion

        return Ok("Operation in compeleted. DO NOT FORGET TO CHANGE ADMIn`S PASSWORD!!!");
    }
}