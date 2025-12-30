using Microsoft.AspNetCore.Authorization;

namespace api.Repositories;

[Authorize]
public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<Gamer> _collection;
    private readonly ITokenService _tokenService;
    private readonly IPhotoService _photoService;

    public UserRepository(IMongoClient client, IMongoDbSettings dbSettings, ITokenService tokenService, IPhotoService photoService)
    {
        var dbName = client.GetDatabase(dbSettings.DatabaseName);
        _collection = dbName.GetCollection<Gamer>("gamers");

        _tokenService = tokenService;
        _photoService = photoService;
    }

    public async Task<Gamer?> GetByIdAsync(string userId, CancellationToken cancellationToken)
    {
        Gamer? gamer = await _collection.Find(doc => doc.Id == userId).SingleOrDefaultAsync(cancellationToken);

        if (gamer is null) return null;

        return gamer;
    }
    public async Task<MemberDto?> UpdateResultAsync(string userId, Gamer userInput, CancellationToken cancellationToken)
    {
        Gamer gamer = await _collection.Find<Gamer>(doc => doc.Id == userId).FirstOrDefaultAsync(cancellationToken);

        if (gamer is null) return null;

        UpdateDefinition<Gamer> updateDef = Builders<Gamer>.Update
        .Set(user => user.Email, userInput.Email.Trim().ToLower())
        .Set(user => user.Name, userInput.Name)
        .Set(user => user.Country, userInput.Country)
        .Set(user => user.City, userInput.City);

        await _collection.UpdateOneAsync(user => user.Id == userId, updateDef, null, cancellationToken);

        Gamer user = await _collection.Find<Gamer>(doc => doc.Id == userId).FirstOrDefaultAsync(cancellationToken);

        if (user is null) return null;

        return Mappers.ConvertGamerToMemberDto(user);
    }

    public async Task<Photo?> UploadPhotoAsync(IFormFile file, string userId, CancellationToken cancellationToken)
    {
        Gamer? user = await GetByIdAsync(userId, cancellationToken);

        if (user is null) return null;

        Photo oldPhoto = user.Photo;

        if (oldPhoto is null)
        {
            Gamer? gamer1 = await GetByIdAsync(userId, cancellationToken);

            if (gamer1 is null) return null;

            ObjectId objectId1 = ObjectId.Parse(userId);

            string[]? imageUrls1 = await _photoService.AddPhotoToDiskAsync(file, objectId1);

            if (imageUrls1 is not null)
            {
                Photo newPhoto;

                newPhoto = Mappers.ConvertPhotoUrlsToPhoto(imageUrls1, true);

                UpdateDefinition<Gamer> updateUser = Builders<Gamer>.Update.Set(doc => doc.Photo, newPhoto);

                UpdateResult result = await _collection.UpdateOneAsync(doc => doc.Id == userId, updateUser, null, cancellationToken);

                return result.ModifiedCount == 1 ? newPhoto : null;
            }

            return null;
        }

        bool isDeleteSuccess = await _photoService.DeletePhotoFromDisk(oldPhoto);
        if (!isDeleteSuccess) return null;

        if (user is null) return null;

        ObjectId objectId = ObjectId.Parse(userId);

        string[]? imageUrls = await _photoService.AddPhotoToDiskAsync(file, objectId);

        if (imageUrls is not null)
        {
            Photo newPhoto;

            newPhoto = Mappers.ConvertPhotoUrlsToPhoto(imageUrls, true);

            UpdateDefinition<Gamer> updateUser = Builders<Gamer>.Update.Set(doc => doc.Photo, newPhoto);

            UpdateResult result = await _collection.UpdateOneAsync(doc => doc.Id == userId, updateUser, null, cancellationToken);

            return result.ModifiedCount == 1 ? newPhoto : null;
        }

        return null;
    }
}