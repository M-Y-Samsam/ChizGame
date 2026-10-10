using api.Enums;
using api.Helpers;
using api.Models.Helpers;
using Image_Processing_WwwRoot.Helpers;
using MongoDbGenericRepository.Utils;

namespace api.Repositories;

public class GameRepository : IGameRepository
{
    #region Mongodb
    private readonly IMongoCollection<Games> _collection;
    private readonly IPhotoService _photoService;
    private readonly ITokenService _tokenService;
    private readonly ILogger<GameRepository> _logger;

    // Dependency Injection
    public GameRepository(IMongoClient client, IMyMongoDbSettings dbSettings, ITokenService tokenService, IPhotoService photoService, ILogger<GameRepository> logger)
    {
        var dbName = client.GetDatabase(dbSettings.DatabaseName);
        _collection = dbName.GetCollection<Games>("games");

        _photoService = photoService;
        _tokenService = tokenService;
        _logger = logger;
    }
    #endregion
    public async Task<Games?> AddGameAsync(GameInfoDto input, CancellationToken cancellationToken)
    {
        Games? game1 = await _collection.Find<Games>(doc => doc.Slug == input.Slug).FirstOrDefaultAsync(cancellationToken);
        Games? game2 = await _collection.Find<Games>(doc => doc.Title == input.Title).FirstOrDefaultAsync(cancellationToken);

        if (game1 is not null && game2 is not null) return null;

        Games games = Mappers.ConverGameInfoToGames(input);

        await _collection.InsertOneAsync(games, null, cancellationToken);

        return games;
    }

    public async Task<GamePhoto?> UploadGamePhotoAsync(IFormFile formFile, PhotoType photoType, string gameId, CancellationToken cancellationToken)
    {
        Games? games = await _collection.Find(doc => doc.Id == gameId).FirstOrDefaultAsync(cancellationToken);

        if (games is null) return null;

        if (!ObjectId.TryParse(gameId, out var objectId)) return null;

        string[]? imageUrls = photoType switch
        {
            PhotoType.Cover =>
                await _photoService.AddCoverToDiskAsync(formFile, objectId),

            PhotoType.Banner =>
                await _photoService.AddBannerToDiskAsync(formFile, objectId),

            PhotoType.Logo =>
                await _photoService.AddLogoToDiskAsync(formFile, objectId),

            PhotoType.ScreenShots =>
                await _photoService.AddScreenshotsToDiskAsync(formFile, objectId),

            _ => null
        };

        if (imageUrls is not null)
        {
            GamePhoto gamePhoto;

            gamePhoto = Mappers.ConvertImageUrlsToGamePhoto(imageUrls, photoType);

            switch (photoType)
            {
                case PhotoType.Cover:
                    UpdateDefinition<Games> updateCover = Builders<Games>.Update.Set(doc => doc.CoverImage, gamePhoto);
                    UpdateResult updateCoverResult = await _collection.UpdateOneAsync(doc => doc.Id == gameId, updateCover, null, cancellationToken);
                    return updateCoverResult.ModifiedCount == 1 ? gamePhoto : null;

                case PhotoType.Banner:
                    UpdateDefinition<Games> updateBanner = Builders<Games>.Update.Set(doc => doc.BannerImage, gamePhoto);
                    UpdateResult updateBannerResult = await _collection.UpdateOneAsync(doc => doc.Id == gameId, updateBanner, null, cancellationToken);
                    return updateBannerResult.ModifiedCount == 1 ? gamePhoto : null;

                case PhotoType.Logo:
                    UpdateDefinition<Games> updateLogo = Builders<Games>.Update.Set(doc => doc.LogoImage, gamePhoto);
                    UpdateResult updateLogoResult = await _collection.UpdateOneAsync(doc => doc.Id == gameId, updateLogo, null, cancellationToken);
                    return updateLogoResult.ModifiedCount == 1 ? gamePhoto : null;

                case PhotoType.ScreenShots:
                    UpdateDefinition<Games> updateScreenshots = Builders<Games>.Update.Push(doc => doc.Screenshots, gamePhoto);
                    UpdateResult updateScreenshotsResult = await _collection.UpdateOneAsync(doc => doc.Id == gameId, updateScreenshots, null, cancellationToken);
                    return updateScreenshotsResult.ModifiedCount == 1 ? gamePhoto : null;
            }
        }

        return null;
    }

    public Task<PagedList<Games>> GetAllGamesAsync(GameParams gameParams, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}