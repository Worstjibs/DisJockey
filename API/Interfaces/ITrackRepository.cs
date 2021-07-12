using System.Collections.Generic;
using System.Threading.Tasks;
using API.Helpers;
using API.Entities;
using API.DTOs.Track;

namespace API.Interfaces {
    public interface ITrackRepository {
        Task<Track> GetTrackByIdAsync(int id);
        Task<Track> GetTrackByYoutubeIdAsync(string youtubeId);
        Task<PagedList<TrackListDto>> GetTracks(PaginationParams paginationParams);
        void AddTrack(Track track);
    }
}