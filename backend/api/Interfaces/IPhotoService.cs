namespace api.Interfaces;

public interface IPhotoService
{
    public Task<string[]?> AddPhotoToDiskAsync(IFormFile file, Photo photo, ObjectId userId);
    public Task<string[]?> AddCoverToDiskAsync(IFormFile formFile, ObjectId userId);
    public Task<string[]?> AddBannerToDiskAsync(IFormFile formFile, ObjectId userId);
    public Task<string[]?> AddLogoToDiskAsync(IFormFile formFile, ObjectId userId);
    public Task<string[]?> AddScreenshotsToDiskAsync(IFormFile formFile, ObjectId userId);

    public Task<bool> DeletePhotoFromDisk(Photo photo);
}
