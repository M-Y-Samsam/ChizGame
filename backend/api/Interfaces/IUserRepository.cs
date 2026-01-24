namespace api.Interfaces;

public interface IUserRepository
{
    public Task<Gamer?> GetByIdAsync(string userId, CancellationToken cancellationToken);
    public Task<UpdateResult?> UpdateResultAsync(string userId , UserUpdateDto userInput , CancellationToken cancellationToken);
    public Task<Photo?> UploadPhotoAsync(IFormFile file, string userId, CancellationToken cancellationToken);
    public Task<UpdateResult?> DeleltePhotoAsync(string userId, CancellationToken cancellationToken);
}