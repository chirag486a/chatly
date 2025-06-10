using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Chatly.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string? GetUserId(this ClaimsPrincipal user)
    {
        // Try ClaimTypes.NameIdentifier first, then JwtRegisteredClaimNames.Sub as fallback
        return user.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
    }

    public static string? GetUserName(this ClaimsPrincipal user)
    {
        // Use JwtRegisteredClaimNames.Name for username
        return user.FindFirst(JwtRegisteredClaimNames.Name)?.Value;
    }
}