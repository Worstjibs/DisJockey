using System.Threading.Tasks;
using DisJockey.Core;
using Discord.WebSocket;

namespace API.Interfaces {
    public interface IDiscordTrackService {
        Task AddTrackAsync(SocketUser discordUser, string url);
        Task PullUpTrackAsync(SocketUser discordUser, string url, double currentPosition);
    }
}