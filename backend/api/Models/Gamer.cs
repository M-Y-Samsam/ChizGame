namespace api.Models;

public class Gamer
{
    [property: BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string ConfirmPassword { get; init; } = string.Empty;
    public DateOnly DateOfBirth { get; init; }
    public string Gender { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public List<Photo> Photos { get; init;} = [];
}
