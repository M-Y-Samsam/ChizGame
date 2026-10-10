namespace api.DTOs;

public record MemberDto(
    string Name,
    int Age,
    string Gender,
    string City,
    string Country,
    DateTime LastActive,
    Photo Photo,
    List<string> FavoriteGames
);