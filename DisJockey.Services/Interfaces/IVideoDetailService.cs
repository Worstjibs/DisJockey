using System.Threading.Tasks;
using DisJockey.Core;

namespace DisJockey.Services.Interfaces {
    public interface IVideoDetailService {
        Task<Track> GetVideoDetails(Track track);
        Task<Playlist> GetPlaylistDetails(string playlistId);
    }
}