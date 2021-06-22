using System.Threading.Tasks;
using API.Entities;
using Discord.WebSocket;

namespace API.Discord.Interfaces {
    public interface IDiscordTrackService {
        Task AddTrackAsync(SocketUser discordUser, string url);
        Task PullUpTrackAsync(SocketUser discordUser, string url, double currentPosition);
    }
}