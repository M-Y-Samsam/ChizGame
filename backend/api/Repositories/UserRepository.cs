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

    public async Task<Photo?> UploadPhotoAsync(IFormFile file, string userId, CancellationToken cancellationToken)
    {
        Gamer? gamer = await GetByIdAsync(userId, cancellationToken);

        if (gamer is null) return null;

        if(!ObjectId.TryParse(userId, out var objectId)) return null;

        string[]? imageUrls = await _photoService.AddPhotoToDiskAsync(file, gamer.Photo, objectId);

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
    
    public async Task<MemberDto?> DeleltePhotoAsync(string userId, CancellationToken cancellationToken)
    {
       Gamer? gamer = await GetByIdAsync(userId, cancellationToken);

        if ( gamer is null) return null;

        Photo? photo = gamer.Photo;
        
        if ( photo is null) return null;

        bool isDeleteSuccess = await _photoService.DeletePhotoFromDisk(photo);
        if ( !isDeleteSuccess ) return null;

        UpdateDefinition<Gamer> update = Builders<Gamer>.Update.Unset(gamer => gamer.Photo );

        await _collection.UpdateOneAsync<Gamer>(gamer => gamer.Id!.ToString() == userId , update, null , cancellationToken);

        Gamer user = await _collection.Find( doc => doc.Id == userId).FirstOrDefaultAsync(cancellationToken);

        if ( user is null ) return null;

        return Mappers.ConvertGamerToMemberDto(user);
    }
}