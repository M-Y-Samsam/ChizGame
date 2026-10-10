using api.Enums;

namespace api.Helpers;

public class GameParams : PaginationParams
{
    public string? GameId { get; set; }
    [MaxLength(30)]
    public string? OrderBy { get; set; } = "New";
    [MaxLength(100)]
    public string? Search { get; set; } = string.Empty;
    public List<GameTags> GameTags { get; set; } = [];
    public List<Platforms> Platforms { get; set; } = [];
    public GameAgeRange MinAge { get; set; }
    public GameAgeRange MaxAge { get; set; }
    public int? MinSize { get; set; }
    public int? MaxSize { get; set; }
}