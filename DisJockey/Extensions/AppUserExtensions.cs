using DisJockey.Core;
using Discord.WebSocket;

namespace DisJockey.Extensions {
    public static class AppUserExtensions {
        public static void UpdateAppUser(this AppUser user, SocketUser discordUser) {
            user.AvatarUrl = discordUser.GetAvatarUrl();
            user.UserName = discordUser.Username;            
        }
    }
}