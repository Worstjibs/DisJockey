using System.Threading.Tasks;
using DisJockey.Core;

namespace API.Interfaces {
    public interface IVideoDetailService {
        Task<Track> GetVideoDetails(Track track);
        Task<Playlist> GetPlaylistDetails(string playlistId);
    }
}