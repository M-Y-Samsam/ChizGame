using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace api.Models;

[CollectionName("gamers")]
public class Gamer : MongoIdentityUser<ObjectId>
{
    public DateOnly DateOfBirth { get; init; }
    public DateTime LastActive { get; init; }
    public string Gender { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public Photo Photo { get; init; } = default!;
    public List<string> FavoriteGames { get; init; } = [];
}
