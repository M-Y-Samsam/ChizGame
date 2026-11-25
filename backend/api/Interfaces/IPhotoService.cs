namespace api.Interfaces;

public interface IPhotoService
{
    public Task<string[]?> AddPhotoToDiskAsync(IFormFile file, Photo photo, string userId);
    public Task<string[]?> AddCoverToDiskAsync(IFormFile formFile, string userId);
    public Task<string[]?> AddBannerToDiskAsync(IFormFile formFile, string userId);
    public Task<string[]?> AddLogoToDiskAsync(IFormFile formFile, string userId);
    public Task<string[]?> AddScreenshotsToDiskAsync(IFormFile formFile, string userId);
    public Task<bool> DeletePhotoFromDisk(Photo photo);
}
