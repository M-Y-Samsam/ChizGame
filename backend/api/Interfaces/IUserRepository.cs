namespace api.Interfaces;

public interface IUserRepository
{
    public Task<Gamer?> GetByIdAsync(string userId, CancellationToken cancellationToken);
    public Task<MemberDto?> UpdateResultAsync(string userId , Gamer userInput , CancellationToken cancellationToken);
    public Task<Photo?> UploadPhotoAsync(IFormFile file, string userId, CancellationToken cancellationToken);
    public Task<MemberDto?> DeleltePhotoAsync(string userId, CancellationToken cancellationToken);
}