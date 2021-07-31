using System.Security.Claims;

namespace DisJockey.Extensions {
    public static class ClaimsPrincipalExtensions {
        public static string GetUsername(this ClaimsPrincipal user) {
            return user.FindFirst(ClaimTypes.Name)?.Value;
        }

        public static ulong? GetDiscordId(this ClaimsPrincipal user) {
            if (ulong.TryParse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value, out ulong discordId)) {
                return discordId;
            }
            return null;
        }

        public static string GetAvatarUrl(this ClaimsPrincipal user) {
            return user.FindFirst(x => x.Type == "urn:discord:avatar:url")?.Value;
        }
    }
}