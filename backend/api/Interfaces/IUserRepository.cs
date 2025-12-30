namespace api.Interfaces;

public interface IUserRepository
{
    public Task<Gamer?> GetByIdAsync(string userId, CancellationToken cancellationToken);
    public Task<MemberDto?> UpdateResultAsync(string userId , Gamer userInput , CancellationToken cancellationToken);
    public Task<bool> DeletePhotoAsync(string userId, CancellationToken cancellationToken);
    public Task<Photo?> JustUploadPhotoAsync(IFormFile file, string userId, CancellationToken cancellationToken);
    public Task<Photo?> FullUploadPhotoAsync(IFormFile file, string userId, CancellationToken cancellationToken);
}