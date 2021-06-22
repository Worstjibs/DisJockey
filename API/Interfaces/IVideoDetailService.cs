using System.Threading.Tasks;
using API.Models;

namespace API.Interfaces {
    public interface IVideoDetailService {
        Task<Track> GetVideoDetails(Track track);
        Task<Playlist> GetPlaylistDetails(string playlistId);
    }
}