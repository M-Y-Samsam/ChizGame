namespace api.DTOs;

public record RegisterDto(
    [Length(1,30)]
    string Name,
    [MaxLength(50), RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,5})+)$", ErrorMessage = "Bad Email Format.")]
    string Email,
    [Range(typeof(DateOnly), "1900-01-01", "2050-01-01")]
    DateOnly DateOfBirth,
    [Length(3,20)]
    string Gender,
    [DataType(DataType.Password), Length(8,20, ErrorMessage = "min of 8 nd max of 20 chars is required")]
    string Password,
    string ConfirmPassword
);