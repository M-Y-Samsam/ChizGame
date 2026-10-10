using api.Helpers;
using Image_Processing_WwwRoot.Helpers;

namespace api.Interfaces;

public interface IGameRepository
{
    public Task<Games?> AddGameAsync(GameInfoDto input, CancellationToken cancellationToken);
    public Task<GamePhoto?> UploadGamePhotoAsync(IFormFile formFile, PhotoType photoType, string gameId, CancellationToken cancellationToken);
    public Task<PagedList<Games>> GetAllGamesAsync(GameParams gameParams, CancellationToken cancellationToken);
}