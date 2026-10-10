namespace api.Models;

public class Mods
{
    [property: BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string? Id {get; init;}
    public string ModName {get ; init;} = string.Empty;
    public string GameName {get; init;} = string.Empty;
    public string Creator {get; init;} = string.Empty;
}