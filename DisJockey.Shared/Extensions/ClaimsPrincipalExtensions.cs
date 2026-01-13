#nullable enable

using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

namespace DisJockey.Shared.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string? GetUsername(this ClaimsPrincipal user)
    {
        return user.FindFirst(JwtRegisteredClaimNames.PreferredUsername)?.Value;
    }

    public static ulong? GetDiscordId(this ClaimsPrincipal user)
    {
        if (ulong.TryParse(user.FindFirst("discord_id")?.Value, out ulong discordId))
        {
            return discordId;
        }

        return null;
    }

    public static string? GetAvatarUrl(this ClaimsPrincipal user)
    {
        var discordId = GetDiscordId(user);
        if (discordId is null)
        {
            return null;
        }

        var avatarId = user.FindFirst(x => x.Type == "avatar")?.Value;
        if (avatarId is null)
        {
            return null;
        };

        return $"https://cdn.discordapp.com/avatars/{discordId}/{avatarId}";
    }
}