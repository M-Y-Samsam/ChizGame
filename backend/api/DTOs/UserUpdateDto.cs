namespace api.DTOs;

public record UserUpdateDto(
    string Name,
    string Country,
    string City
);