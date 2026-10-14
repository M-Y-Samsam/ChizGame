using api.Enums;

namespace api.Helpers;

public class GameParams : PaginationParams
{
    [MaxLength(30)]
    public string? OrderBy { get; set; } = "newest";
    [MaxLength(30)]
    public string? ThenBy { get; set; } = "topRated";
    [MaxLength(100)]
    public string? Search { get; set; } = string.Empty;
    public List<GameTags> GameTags { get; set; } = [];
    public string? TagMatchMode { get; set; } = "OR";
    public List<Platforms> Platforms { get; set; } = [];
    public string? PlatformMatchMode { get; set; } = "OR";
    [Range(3, 18)]
    public GameAgeRange MinAge { get; set; } = GameAgeRange.PGEI3;
    [Range(3, 18)]
    public GameAgeRange MaxAge { get; set; } = GameAgeRange.PGEI18;
    [Range(0, 1000)]
    public int? MinSize { get; set; } = 0;
    [Range(0, 1000)]
    public int? MaxSize { get; set; } = 1000;
}