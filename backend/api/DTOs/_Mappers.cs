namespace api.DTOs;

public static class Mappers
{
    public static Gamer ConvertRegisterDtoToGamer(RegisterDto registerDto)
    {
        Gamer gamer = new Gamer()
        {
            Name = registerDto.Name,
            Email = registerDto.Email,
            Password = registerDto.Password,
            ConfirmPassword = registerDto.ConfirmPassword,
            DateOfBirth = registerDto.DateOfBirth,          
        };

        return gamer;
    }

    public static LoggedInDto ConvertGamerToLoggedInDto(Gamer gamer, string tokenValue)
    {
        return new (
            Email: gamer.Email,
            Name: gamer.Name,
            Age: DateTimeExtension.CalculateAge(gamer.DateOfBirth),
            Token: tokenValue
        );
    }

    public static MemberDto ConvertGamerToMemberDto(Gamer gamer)
    {
        return new (
            Email: gamer.Email,
            Name: gamer.Name,
            Age: DateTimeExtension.CalculateAge(gamer.DateOfBirth),
            Gender: gamer.Gender,
            City: gamer.City,
            Country: gamer.Country
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
}