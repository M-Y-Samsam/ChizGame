namespace api.DTOs;

public static class Mappers
{
    public static LoggedInDto ConvertGamerToLoggedInDto(Gamer gamer, string tokenValue)
    {
        return new (
            Email: gamer.Email,
            Name: gamer.Name,
            Age: gamer.Age,
            Token: tokenValue
        );
    }

    public static MemberDto ConvertGamerToMemberDto(Gamer gamer)
    {
        return new (
            Email: gamer.Email,
            Name: gamer.Name,
            Age: gamer.Age,
            Gender: gamer.Gender,
            City: gamer.City,
            Country: gamer.Country
        );
    }
}