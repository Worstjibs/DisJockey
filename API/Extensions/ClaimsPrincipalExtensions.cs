using System.Security.Claims;

namespace API.Extensions {
    public static class ClaimsPrincipalExtensions {
        public static string GetUsername(this ClaimsPrincipal user) {
            return user.FindFirst(ClaimTypes.Name)?.Value;
        }

        public static string GetDiscordId(this ClaimsPrincipal user) {
            return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        public static string GetAvatarUrl(this ClaimsPrincipal user) {
            return user.FindFirst(x => x.Type == "urn:discord:avatar:url")?.Value;
        }
    }
}