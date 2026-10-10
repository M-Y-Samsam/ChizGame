using api.Enums;
using api.Models;
using api.Models.Helpers;
namespace api.Models;

public class Games
{

    [property: BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; init; } // Game Id in Database
    public string Title { get; init; } = string.Empty; // Game Name
    public string Slug { get; init; } = string.Empty; // Clean Name For Url

    public string ShortDes { get; init; } = string.Empty;
    public string FullDes { get; init; } = string.Empty;
    public int MainStoryHours { get; init; }
    public int CompletionistHours { get; init; }
    public bool IsGamePlayableAfterEnding { get; init; }

    public int CriticRating { get; init; }

    public GamePhoto CoverImage { get; init; } = null!; // the image that is in get all games page
    public GamePhoto BannerImage { get; init; } = null!; // main image in game page
    public GamePhoto LogoImage { get; init; } = null!;
    public List<GamePhoto> Screenshots { get; init; } = []; // Gameplay images

    public string TrailerUrl { get; init; } = string.Empty; // Game Trailer Url

    public List<GameTags> GameTags { get; init; } = []; // Game Tags And Categorys

    public string Developer { get; init; } = string.Empty; // Game Compony
    public string Publisher { get; init; } = string.Empty; // Game Publisher
    public DateOnly ReleaseDate { get; init; } // Date of Releasing Game
    public GameAgeRange MinimumAge { get; init; } // gmae minimum age 

    public List<Platforms> Platforms { get; init; } = []; // Platforms supported by this game

    public GameRequirement MinimumRequirements { get; init; } = new(); // Minimum system requirements for this game
    public GameRequirement MaximumRequirements { get; init; } = new(); // Maximum system requirements for this game

    public List<GameDownloadLink> DownloadLinks { get; init; } = [];
}
