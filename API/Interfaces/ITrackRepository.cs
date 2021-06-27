using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Helpers;
using API.Entities;
using API.Models;

namespace API.Interfaces {
    public interface ITrackRepository {
        Task<Track> GetTrackByIdAsync(int id);
        Task<Track> GetTrackByYoutubeIdAsync(string youtubeId);
        Task<PagedList<TrackDto>> GetTracks(PaginationParams paginationParams);
        void AddTrack(Track track);
        Task AddMissingTracks(IList<PlaylistTrack> playlistTracks);
        Task AddPlaylist(Playlist playlist);
        Task<PlaylistModel> GetPlaylist(int playlistId);
    }
}