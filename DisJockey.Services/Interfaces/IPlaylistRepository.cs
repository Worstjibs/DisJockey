using DisJockey.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using DisJockey.Shared.Helpers;
using DisJockey.Shared.DTOs.Shared;

namespace DisJockey.Services.Interfaces {
    public interface IPlaylistRepository {
        Task<Playlist> AddPlaylist(Playlist playlist, AppUser user);
        Task AddMissingTracks(IList<PlaylistTrack> playlistTracks);
        Task<PagedList<BaseTrackDto>> GetPlaylistTracks(PaginationParams paginationParams, string youtubeId);
        Task<bool> CheckPlaylistExists(string playlistId);
    }
}
