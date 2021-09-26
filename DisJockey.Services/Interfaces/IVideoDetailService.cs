using System.Collections.Generic;
using System.Threading.Tasks;
using DisJockey.Core;
using DisJockey.Shared.Helpers;

namespace DisJockey.Services.Interfaces {
    public interface IVideoDetailService {
        Task<Track> GetVideoDetailsAsync(Track track);
        Task<Playlist> GetPlaylistDetailsAsync(string playlistId);
        Task<YouTubePagedList<Track>> QueryTracksAsync(PaginationParams paginationParams);
    }
}