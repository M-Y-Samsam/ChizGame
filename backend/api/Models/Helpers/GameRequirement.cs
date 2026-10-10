namespace api.Models.Helpers;

// public record GameRequirement
// (
//     string OS,
//     int Ram,
//     string CPU,
//     string GPU,
//     string Storage
// );
public class GameRequirement
{
    public string OS { get; init; } = string.Empty;
    public int Ram { get; init; }
    public string CPU { get; init;} = string.Empty;
    public string GPU { get; init; } = string.Empty;
    public string Storage { get; init; } = string.Empty;
}