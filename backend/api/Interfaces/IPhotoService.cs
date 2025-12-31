namespace api.Interfaces;

public interface IPhotoService
{
    public Task<string[]?> AddPhotoToDiskAsync(IFormFile file, Photo photo, ObjectId userId);

    public Task<bool> DeletePhotoFromDisk(Photo photo);
}
