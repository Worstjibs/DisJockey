using System.Threading.Tasks;
using API.Models;

namespace API.Discord.Interfaces {
    public interface IDiscordTrackService {
        Task<bool> AddTrackAsync(ulong discordId, string username, string url);
    }
}