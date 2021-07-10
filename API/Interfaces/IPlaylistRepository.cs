using API.DTOs.Playlist;
using API.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Interfaces {
    public interface IPlaylistRepository {
        Task<PlaylistDto> GetPlaylistById(int id);
        Task<PlaylistDto> GetPlaylistByYoutubeId(string youtubeId);
        Task<Playlist> AddPlaylist(Playlist playlist, AppUser user);
        Task AddMissingTracks(IList<PlaylistTrack> playlistTracks);
        Task<bool> CheckPlaylistExists(string playlistId);
    }
}
