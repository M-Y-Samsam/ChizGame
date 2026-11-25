namespace api.Interfaces;

public interface IUserRepository
{
    public Task<UpdateResult?> UpdateByIdAsync(string userIdHashed, UserUpdateDto userInput, CancellationToken cancellationToken);
    public Task<Photo?> UploadPhotoAsync(IFormFile file, string userIdHashed, CancellationToken cancellationToken);
    public Task<UpdateResult?> DeleltePhotoAsync(string userIdHashed, CancellationToken cancellationToken);
}