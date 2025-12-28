namespace api.DTOs;

public record RegisterDto(
    string Name,
    string Email,
    DateOnly DateOfBirth,
    string Password,
    string ConfirmPassword
);