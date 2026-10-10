using api.Enums;
using Image_Processing_WwwRoot.Helpers;

namespace api.Models;

public record GamePhoto(
    string SmallPhotoUrl,
    string? MediumPhotoUrl,
    string BigPhotoUrl,
    PhotoType PhotoType
);