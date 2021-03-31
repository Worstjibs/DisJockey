using System.Threading.Tasks;
using API.Entities;

namespace API.Discord.Interfaces {
    public interface IDiscordTrackService {
        Task<AppUserTrack> AddTrackAsync(long discordId, string url);
    }
}