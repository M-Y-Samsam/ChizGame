using System.Linq.Expressions;
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

    public async Task<Photo?> FullUploadPhotoAsync(IFormFile file, string userId, CancellationToken cancellationToken)
    {
        Gamer? user = await GetByIdAsync(userId, cancellationToken);

        if (user is null) return null;

        Photo oldPhoto = user.Photo;

        if (oldPhoto is null)
        {
            return await JustUploadPhotoAsync(file, userId, cancellationToken);
        }

        bool isDeleteSuccess = await DeletePhotoAsync(userId, cancellationToken);
        if (isDeleteSuccess is false) return null;


        return await JustUploadPhotoAsync(file, userId, cancellationToken);
    }

    public async Task<bool> DeletePhotoAsync(string userId, CancellationToken cancellationToken)
    {
        Gamer? user = await GetByIdAsync(userId, cancellationToken);

        if (user is null) return false;

        Photo oldPhoto = user.Photo;

        bool isDeleteSuccess = await _photoService.DeletePhotoFromDisk(oldPhoto);

        return isDeleteSuccess;
    }

    public async Task<Photo?> JustUploadPhotoAsync(IFormFile file, string userId, CancellationToken cancellationToken)
    {
        Gamer? gamer = await GetByIdAsync(userId, cancellationToken);

        if (gamer is null) return null;

        ObjectId.TryParse(userId, out var objectId);

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

    public async Task<MemberDto?> RemovePhotoAsync(string userId, CancellationToken cancellationToken)
    {
        Gamer? gamer = await GetByIdAsync(userId, cancellationToken);

        if ( gamer is null) return null;

        Photo? photo = gamer.Photo;
        
        if ( photo is null) return null;

        bool isDeleteSuccess = await _photoService.DeletePhotoFromDisk(photo);
        if ( !isDeleteSuccess ) return null;

        UpdateDefinition<Gamer> update = Builders<Gamer>.Update.Inc(gamer => gamer.Photo , photo );

        await _collection.UpdateOneAsync<Gamer>(gamer => gamer.Id!.ToString() == userId , update, null , cancellationToken);

        Gamer user = await _collection.Find( doc => doc.Id == userId).FirstOrDefaultAsync(cancellationToken);

        if ( user is null ) return null;

        return Mappers.ConvertGamerToMemberDto(user);
    }
}