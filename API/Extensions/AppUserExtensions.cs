using API.Entities;
using Discord.WebSocket;

namespace API.Extensions {
    public static class AppUserExtensions {
        public static void UpdateAppUser(this AppUser user, SocketUser discordUser) {
            user.AvatarUrl = discordUser.GetAvatarUrl();
            user.UserName = discordUser.Username;            
        }
    }
}