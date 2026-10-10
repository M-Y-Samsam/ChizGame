using api.Enums;
using Image_Processing_WwwRoot.Helpers;
using Image_Processing_WwwRoot.Interfaces;

namespace api.Services;

public class PhotoService(
    IPhotoModifySaveService _photoModifyService,
    ILogger<PhotoService> _logger
    ) : PhotoStandardSize, IPhotoService
{
    #region Constructor and variables

    const string wwwRootUrl = "wwwroot/";

    #endregion

    /// <summary>
    /// ADD PHOTO TO DISK
    /// Store photos on disk by creating a folder based on fileName, userId, size, crop, etc. Each user will have a folder named after their db _Id.
    /// Resize and square image with 165px(navbar & thumbnail), 256px(card).
    /// Scale image to a ~300kb max size for the enlarged gallery photo.
    /// Store photo address in db as "storage/photos/user-id/resize-pixel-square/128x128/my-photo.jpg"
    /// 
    /// DELETE PHOTO FROM DISK
    /// </summary>
    /// <param name="formFile"></param>
    /// <param name="userId"></param>
    /// <returns>ADD: array of filePaths. DELETE: boolean</returns>

    #region UserProfile
    public async Task<string[]?> AddPhotoToDiskAsync(IFormFile formFile, Photo photo, ObjectId userId)
    {
        if (photo is not null)
        {
            await DeletePhotoFromDisk(photo);
        }

        // copy file/s to the folder
        if (formFile.Length > 0) // 301kb => 301_000 byte
        {
            #region Resize and/or Store Images to Disk
            // await _photoModifyService.Crop(formFile, userId, 450, 800);
            // await _photoModifyService.Crop_Square(formFile, userId, 400);
            // await _photoModifyService.CropWithOriginalSide_Square(formFile, userId);
            // await _photoModifyService.ResizeByPixel(formFile, userId, 500, 800);
            // await _photoModifyService.ResizeByPixel_Square(formFile, userId, 500);
            // await _photoModifyService.ResizeImageByScale(formFile, userId);

            string filePath_165_sq = await _photoModifyService.ResizeByPixel_Square(formFile, userId, 165, PhotoCategory.UserProfile); // navbar & thumbnail
            string filePath_256_sq = await _photoModifyService.ResizeByPixel_Square(formFile, userId, 256, PhotoCategory.UserProfile); // card
            string filePath_enlarged = await _photoModifyService.ResizeImageByScale(formFile, userId, (int)DimensionsEnum._4_3_800x600, PhotoCategory.UserProfile); // enlarged photo
            // string filePath_enlarged2 = await _photoModifyService.ResizeImageByScale(formFile, userId, (int)DimensionsEnum._4_3_1280x960); // enlarged photo

            // if conversion fails
            if (filePath_165_sq is null || filePath_256_sq is null || filePath_enlarged is null)
            {
                _logger.LogError("Photo addition failed. The returned filePath is null which is not allowed.");
                return null;
            }
            #endregion Resize and Create Images to Disk

            #region Create the photo URLs and return the result
            // // generate "wwwroot/storage/photos/user-id/resize-pixel-square/128x128/my-photo.jpg"
            return [
                filePath_165_sq.Split(wwwRootUrl)[1], // 0
                filePath_256_sq.Split(wwwRootUrl)[1], // 1
                filePath_enlarged.Split(wwwRootUrl)[1] // 2

                // string name = "amirRshaghaghi";
                // string[] names = name.Split("R"); // ["amir", "shaghaghi"];
                // names[0] // "amir"
            ];
            #endregion
        }

        return null;
    }
    #endregion UserProfile

    #region Cover
    public async Task<string[]?> AddCoverToDiskAsync(IFormFile formFile, ObjectId gameId)
    {
        if (formFile.Length > 0)
        {
            #region Resize and/or store images to disk

            string filePath_small = await _photoModifyService.ResizeByPixel(formFile, gameId, 400, 600, PhotoCategory.Game, PhotoType.Cover);
            string filePath_big = await _photoModifyService.ResizeByPixel(formFile, gameId, 600, 900, PhotoCategory.Game, PhotoType.Cover);

            if (filePath_small is null || filePath_big is null)
            {
                _logger.LogError("Photo addition failed. The returned filePath is null which is not allowed.");
                return null;
            }
            #endregion Resize and/or store images to disk

            #region Create the photo URLs and return the result
            return [
                filePath_small.Split(wwwRootUrl)[1],
                filePath_big.Split(wwwRootUrl)[1]
            ];
            #endregion Create the photo URLs and return the result
        }

        return null;
    }
    #endregion Cover

    #region Banner
    public async Task<string[]?> AddBannerToDiskAsync(IFormFile formFile, ObjectId gameId)
    {
        if (formFile.Length > 0)
        {
            #region Resiza and/or store images to disk

            string filePath_small = await _photoModifyService.ResizeByPixel(formFile, gameId, 360, 640, PhotoCategory.Game, PhotoType.Banner);
            string filePath_medium = await _photoModifyService.ResizeByPixel(formFile, gameId, 720, 1280, PhotoCategory.Game, PhotoType.Banner);
            string filePath_big = await _photoModifyService.ResizeByPixel(formFile, gameId, 1080, 1920, PhotoCategory.Game, PhotoType.Banner);

            if (filePath_small is null || filePath_medium is null || filePath_big is null)
            {
                _logger.LogError("Photo addition failed. the returned filePath is null witch is not allowed.");
                return null;
            }
            #endregion Resiza and/or store images to disk

            #region Create the photo URLs and return the result
            return [
                filePath_small.Split(wwwRootUrl)[1],
                filePath_medium.Split(wwwRootUrl)[1],
                filePath_big.Split(wwwRootUrl)[1]
            ];
            #endregion Create the photo URLs and return the result            
        }

        return null;
    }
    #endregion Banner

    #region Logo
    public async Task<string[]?> AddLogoToDiskAsync(IFormFile formFile, ObjectId gameId)
    {
        if (formFile.Length > 0)
        {
            #region  Resize and/or store images to disk

            string filePath_small = await _photoModifyService.ResizeByPixel(formFile, gameId, 250, 600, PhotoCategory.Game, PhotoType.Logo);
            string filePath_big = await _photoModifyService.ResizeByPixel(formFile, gameId, 500, 1200, PhotoCategory.Game, PhotoType.Logo);

            if (filePath_small is null || filePath_big is null)
            {
                _logger.LogError("Photo addition is failed. the returned filePath is null wich is not allowed.");
                return null;
            }
            #endregion Resize and/or store images to disk

            #region Create photo URLs and return the result
            return [
                filePath_small.Split(wwwRootUrl)[1],
                filePath_big.Split(wwwRootUrl)[1]
            ];
            #endregion Create photo URLs and return the result
        }

        return null;
    }
    #endregion Logo

    #region ScreenShots
    public async Task<string[]?> AddScreenshotsToDiskAsync(IFormFile formFile, ObjectId gameId)
    {
        if (formFile.Length > 0)
        {
            #region Resiza and/or store images to disk

            string filePath_small = await _photoModifyService.ResizeByPixel(formFile, gameId, 180, 320, PhotoCategory.Game, PhotoType.ScreenShots);
            string filePath_medium = await _photoModifyService.ResizeByPixel(formFile, gameId, 720, 1280, PhotoCategory.Game, PhotoType.ScreenShots);
            string filePath_big = await _photoModifyService.ResizeByPixel(formFile, gameId, 1080, 1920, PhotoCategory.Game, PhotoType.ScreenShots);

            if (filePath_small is null || filePath_medium is null || filePath_big is null)
            {
                _logger.LogError("Photo addition failed. the returned filePath is null wich is not allowed.");
                return null;
            }
            #endregion Resiza and/or store images to disk

            #region Create photo URLs and return the result
            return [
                filePath_small.Split(wwwRootUrl)[1],
                filePath_medium.Split(wwwRootUrl)[1],
                filePath_big.Split(wwwRootUrl)[1]
            ];
            #endregion Create photo URLs and return the result
        }

        return null;
    }
    #endregion ScreenShots

    /// <summary>
    /// Delete all files of the requested photo to be deleted.
    /// </summary>
    /// <param name="photo"></param>
    /// <returns>bool</returns>
    public async Task<bool> DeletePhotoFromDisk(Photo photo)
    {
        List<string> photoPaths = [];

        photoPaths.Add(photo.Url_165);
        photoPaths.Add(photo.Url_256);
        photoPaths.Add(photo.Url_enlarged);

        foreach (string photoPath in photoPaths)
        {
            if (File.Exists(wwwRootUrl + photoPath))
            {
                // Delete the file on a background thread and await the task
                await Task.Run(() => File.Delete(wwwRootUrl + photoPath));
            }
            else
                return false;
        }

        return true;
    }
}