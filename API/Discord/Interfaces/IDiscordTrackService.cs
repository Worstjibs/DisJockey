using System.Threading.Tasks;
using API.Models;
using Discord.WebSocket;

namespace API.Discord.Interfaces {
    public interface IDiscordTrackService {
        Task<bool> AddTrackAsync(SocketUser discordUser, string url);
        Task<bool> PullUpTrackAsync(SocketUser discordUser, string url);
    }
}