using System.Security.Claims;

namespace api.Extensions;

public static class ClaimPrincipalExtension
{
    public static string? GetUserId(this ClaimsPrincipal usser)
    {
        return usser.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}