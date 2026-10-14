using api.Enums;
using api.Models.Helpers;

namespace api.DTOs;

public record GameInfoDto(
    string Title,
    string Slug,
    string ShortDes,
    string FullDes,
    int MainStoryHours,
    int CompletionistHours,
    bool IsGamePlayableAfterEnding,
    int CriticRating,
    string TrailerUrl,
    List<GameTags> GameTags,
    string Developer,
    string Publisher,
    DateOnly ReleaseDate,
    GameAgeRange MinimumAge,
    List<Platforms> Platforms,
    GameRequirement MinimumRequirement,
    GameRequirement MaximumRequirement,
    double Size,
    List<GameDownloadLink> DownloadLinks
);