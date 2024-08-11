using DisJockey.Core;
using DisJockey.Shared.Helpers;

namespace DisJockey.Application.Interfaces;

public interface IVideoDetailService
{
    Task<Track> GetVideoDetailsAsync(Track track);
    Task<Playlist?> GetPlaylistDetailsAsync(string playlistId);
    Task<YouTubePagedList<Track>> QueryTracksAsync(PaginationParams paginationParams);
}