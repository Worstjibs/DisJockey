using DisJockey.Core;
using DisJockey.Shared.Helpers;
using DisJockey.Shared.DTOs.Track;
using DisJockey.Application.Interfaces;

namespace DisJockey.Services.Interfaces;

public interface IPlaylistRepository : IBaseRepository
{
    Task<Playlist> AddPlaylist(Playlist playlist, AppUser user);
    Task AddMissingTracks(IList<PlaylistTrack> playlistTracks);
    Task<PagedList<TrackListDto>> GetPlaylistTracks(PaginationParams paginationParams, string youtubeId);
    Task<bool> CheckPlaylistExists(string playlistId);
}
