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
            }
        }

        return null;
    }

    public async Task<List<GamePhoto>?> UploadScreenshotsAsync(IEnumerable<IFormFile> formFiles, PhotoType photoType, string gameId, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(gameId, out var objectId)) return null;

        Games? games = await _collection.Find(doc => doc.Id == gameId).FirstOrDefaultAsync(cancellationToken);

        if (games is null) return null;

        List<GamePhoto> gamePhotos = [];

        foreach (IFormFile formFile in formFiles)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string[]? imageUrls = await _photoService.AddScreenshotsToDiskAsync(formFile, objectId);

            if (imageUrls is null)
            {
                _logger.LogError("Screenshot {FileName} could not processed.", formFile.FileName);
                continue;
            }

            gamePhotos.Add(Mappers.ConvertImageUrlsToGamePhoto(imageUrls, PhotoType.ScreenShots));
        }

        if (gamePhotos.Count == 0) return null;

        UpdateDefinition<Games> update = Builders<Games>.Update.PushEach(doc => doc.Screenshots, gamePhotos);

        UpdateResult result = await _collection.UpdateOneAsync(doc => doc.Id == gameId, update, null, cancellationToken);

        return result.ModifiedCount == 1 ? gamePhotos : null;
    }

    public async Task<PagedList<Games>> GetAllGamesAsync(GameParams gameParams, CancellationToken cancellationToken)
    {
        PagedList<Games> games = await PagedList<Games>.CreatePagedListAsync(CreateQuery(gameParams), gameParams.PageNumber, gameParams.PageSize, cancellationToken);

        return games;
    }

    public IQueryable<Games>? CreateQuery(GameParams gameParams)
    {
        if (gameParams.OrderBy == gameParams.ThenBy)
            return null;

        IQueryable<Games> query = _collection.AsQueryable();

        IOrderedQueryable<Games> orderedQuery = gameParams.OrderBy switch
        {
            GameOrder.Newest => query.OrderByDescending(g => g.ReleaseDate),
            GameOrder.Oldest => query.OrderBy(g => g.ReleaseDate),

            GameOrder.AtoZ => query.OrderBy(g => g.Title),
            GameOrder.ZtoA => query.OrderByDescending(g => g.Title),

            GameOrder.TopRated => query.OrderByDescending(g => g.CriticRating),
            GameOrder.WorstRated => query.OrderBy(g => g.CriticRating),

            GameOrder.MinAgeToMaxAge => query.OrderBy(g => g.MinimumAge),
            GameOrder.MaxAgeToMinAge => query.OrderByDescending(g => g.MinimumAge),

            GameOrder.Smallest => query.OrderBy(g => g.Size),
            GameOrder.Largest => query.OrderByDescending(g => g.Size),

            // Newest
            _ => query.OrderByDescending(g => g.ReleaseDate)
        };

        orderedQuery = gameParams.ThenBy switch
        {
            GameOrder.Newest => orderedQuery.ThenByDescending(g => g.ReleaseDate),
            GameOrder.Oldest => orderedQuery.ThenBy(g => g.ReleaseDate),

            GameOrder.AtoZ => orderedQuery.ThenBy(g => g.Title),
            GameOrder.ZtoA => orderedQuery.ThenByDescending(g => g.Title),

            GameOrder.TopRated => orderedQuery.ThenByDescending(g => g.CriticRating),
            GameOrder.WorstRated => orderedQuery.ThenBy(g => g.CriticRating),

            GameOrder.MinAgeToMaxAge => orderedQuery.ThenBy(g => g.MinimumAge),
            GameOrder.MaxAgeToMinAge => orderedQuery.ThenByDescending(g => g.MinimumAge),

            GameOrder.Smallest => orderedQuery.ThenBy(g => g.Size),
            GameOrder.Largest => orderedQuery.ThenByDescending(g => g.Size),

            // TopRated
            _ => orderedQuery.OrderByDescending(g => g.CriticRating)
        };

        query = orderedQuery;

        if (!string.IsNullOrEmpty(gameParams.Search))
        {
            query = query.Where(
                g => g.Title.Contains(gameParams.Search, StringComparison.CurrentCultureIgnoreCase)
                || g.Developer.Contains(gameParams.Search, StringComparison.CurrentCultureIgnoreCase)
                || g.Publisher.Contains(gameParams.Search, StringComparison.CurrentCultureIgnoreCase)
            );
        }

        if (gameParams.GameTags.Count > 0)
        {
            query = gameParams.TagMatchMode switch
            {
                "AND" => query.Where(g => gameParams.GameTags.All(selectedTag => g.GameTags.Contains(selectedTag))),

                // OR
                _ => query.Where(g => gameParams.GameTags.Any(tag => g.GameTags.Contains(tag)))
            };
        }

        if (gameParams.Platforms.Count > 0)
        {
            query = gameParams.PlatformMatchMode switch
            {
                "AND" => query.Where(g => gameParams.Platforms.All(selectedPlatforms => g.Platforms.Contains(selectedPlatforms))),

                // OR
                _ => query.Where(g => gameParams.Platforms.Any(platforms => g.Platforms.Contains(platforms)))
            };
        }

        query = query.Where(g => g.MinimumAge >= gameParams.MinAge && g.MinimumAge <= gameParams.MaxAge);
        query = query.Where(g => g.Size >= gameParams.MinSize && g.Size <= gameParams.MaxSize);

        return query;
    }
}