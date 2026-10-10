using api.Models.Helpers;
using Image_Processing_WwwRoot.Helpers;

namespace api.DTOs;

public static class Mappers
{
    public static Games ConverGameInfoToGames(GameInfoDto gameInfoDto)
    {
        Games games = new Games()
        {
            Title = gameInfoDto.Title.Trim(),
            Slug = gameInfoDto.Slug.Trim(),
            ShortDes = gameInfoDto.ShortDes,
            FullDes = gameInfoDto.FullDes,
            MainStoryHours = gameInfoDto.MainStoryHours,
            CompletionistHours = gameInfoDto.CompletionistHours,
            IsGamePlayableAfterEnding = gameInfoDto.IsGamePlayableAfterEnding,
            CriticRating = gameInfoDto.CriticRating,
            TrailerUrl = gameInfoDto.TrailerUrl,
            GameTags = gameInfoDto.GameTags,
            Developer = gameInfoDto.Developer,
            Publisher = gameInfoDto.Publisher,
            MinimumAge = gameInfoDto.MinimumAge,
            Platforms = gameInfoDto.Platforms,
            MinimumRequirements = gameInfoDto.MinimumRequirement,
            MaximumRequirements = gameInfoDto.MaximumRequirement,
            DownloadLinks = gameInfoDto.DownloadLinks
        };

        return games;
    }
    public static Gamer ConvertRegisterDtoToGamer(RegisterDto registerDto)
    {
        Gamer gamer = new Gamer()
        {
            UserName = registerDto.Name.Trim().ToLower(),
            Email = registerDto.Email.Trim().ToLower(),
            DateOfBirth = registerDto.DateOfBirth,
            Gender = registerDto.Gender,
        };

        return gamer;
    }

    public static LoggedInDto ConvertGamerToLoggedInDto(Gamer gamer, string tokenValue)
    {
        return new LoggedInDto
        {
            Email = gamer.Email,
            UserName = gamer.UserName,
            Age = DateTimeExtension.CalculateAge(gamer.DateOfBirth),
            Token = tokenValue,
            ProfilePhotoUrl = gamer.Photo?.Url_165
        };
    }

    public static MemberDto ConvertGamerToMemberDto(Gamer gamer)
    {
        return new(
            Name: gamer.UserName!,
            Age: DateTimeExtension.CalculateAge(gamer.DateOfBirth),
            Gender: gamer.Gender,
            City: gamer.City,
            Country: gamer.Country,
            Photo: gamer.Photo,
            FavoriteGames: gamer.FavoriteGames,
            LastActive: gamer.LastActive
        );
    }

    public static Photo ConvertPhotoUrlsToPhoto(string[] photoUrls, bool isMain)
    {
        return new Photo(
            Url_165: photoUrls[0],
            Url_256: photoUrls[1],
            Url_enlarged: photoUrls[2],
            IsMain: isMain
        );
    }

    public static GamePhoto ConvertImageUrlsToGamePhoto(string[] imageUrls, PhotoType photoType)
    {
        if (imageUrls.Length == 2)
        {
            return new GamePhoto(
                SmallPhotoUrl: imageUrls[0],
                MediumPhotoUrl: null,
                BigPhotoUrl: imageUrls[1],
                PhotoType: photoType
            );
        }

        return new GamePhoto(
            SmallPhotoUrl: imageUrls[0],
            MediumPhotoUrl: imageUrls[1],
            BigPhotoUrl: imageUrls[2],
            PhotoType: photoType
        );
    }
}