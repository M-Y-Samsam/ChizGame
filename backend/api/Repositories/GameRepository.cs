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

    public async Task<PagedList<Games>> GetAllGamesAsync(GameParams gameParams, CancellationToken cancellationToken)
    {
        PagedList<Games> games = await PagedList<Games>.CreatePagedListAsync(CreateQuery(gameParams), gameParams.PageSize, gameParams.PageNumber, cancellationToken);

        return games;
    }

    public IQueryable<Games>? CreateQuery(GameParams gameParams)
    {
        if (gameParams.OrderBy == gameParams.ThenBy)
            return null;

        IQueryable<Games> query = _collection.AsQueryable();

        IOrderedQueryable<Games> orderedQuery = gameParams.OrderBy switch
        {
            "oldest" => query.OrderBy(g => g.ReleaseDate),

            "A-Z" => query.OrderBy(g => g.Title),
            "Z-A" => query.OrderByDescending(g => g.Title),

            "topRated" => query.OrderByDescending(g => g.CriticRating),
            "worstRated" => query.OrderBy(g => g.CriticRating),

            "minAgeToMaxAge" => query.OrderBy(g => g.MinimumAge),
            "maxAgeToMinAge" => query.OrderByDescending(g => g.MinimumAge),

            "smallest" => query.OrderBy(g => g.Size),
            "largest" => query.OrderByDescending(g => g.Size),

            // Newest
            _ => query.OrderByDescending(g => g.ReleaseDate)
        };

        orderedQuery = gameParams.ThenBy switch
        {
            "newest" => orderedQuery.ThenByDescending(g => g.ReleaseDate),
            "oldest" => orderedQuery.ThenBy(g => g.ReleaseDate),

            "A-Z" => orderedQuery.ThenBy(g => g.Title),
            "Z-A" => orderedQuery.ThenByDescending(g => g.Title),

            "worstRated" => orderedQuery.ThenBy(g => g.CriticRating),

            "minAgeToMaxAge" => orderedQuery.ThenBy(g => g.MinimumAge),
            "maxAgeToMinAge" => orderedQuery.ThenByDescending(g => g.MinimumAge),

            "smallest" => orderedQuery.ThenBy(g => g.Size),
            "largest" => orderedQuery.ThenByDescending(g => g.Size),

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