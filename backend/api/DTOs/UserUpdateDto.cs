namespace api.DTOs;

public record UserUpdateDto(
    string Name,
    string Email,
    string Country,
    string City,
    List<string> FavoriteGames
);