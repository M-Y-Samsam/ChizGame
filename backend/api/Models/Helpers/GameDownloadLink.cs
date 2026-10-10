using api.Enums;

namespace api.Models.Helpers;

public class GameDownloadLink
{
    public string? RepackName { get; init; }
    public Platforms Platform { get; init; }
    public string DownloadLink { get; init; } = string.Empty;
    public double GameSize { get; init; }
}

// public record GameDownloadLink(
//     string? RepackName,
//     Platforms Platform,
//     string DownloadLink,
//     double GameSize
// );