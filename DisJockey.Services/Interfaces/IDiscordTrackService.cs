using System.Threading.Tasks;
using DisJockey.Core;
using Discord.WebSocket;

namespace DisJockey.Services.Interfaces;

public interface IDiscordTrackService
{
    Task AddTrackAsync(SocketUser discordUser, string url);
    Task AddTrackAsync(AddTrackArgs args, string youtubeId);
    Task PullUpTrackAsync(SocketUser discordUser, string url, double currentPosition);

    public struct AddTrackArgs
    {
        public ulong DiscordId { get; set; }
        public string Username { get; set; }
        public string AvatarUrl { get; set; }
    }
}