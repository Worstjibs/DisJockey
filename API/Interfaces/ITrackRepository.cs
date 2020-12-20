using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;

namespace API.Interfaces {
    public interface ITrackRepository {
        Task<Track> GetTrackByIdAsync(int id);
        Task<Track> GetTrackByYoutubeIdAsync(string youtubeId);
        Task<IEnumerable<TrackUsersDto>> GetTracksAsync();

        // Task<int> GetTrackLikes(Track track, bool liked);
        Task<bool> AddTrackLike(Track track, AppUser user, bool liked);
    }
}