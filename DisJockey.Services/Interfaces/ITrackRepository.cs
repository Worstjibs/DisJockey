using System.Threading.Tasks;
using DisJockey.Core;
using DisJockey.Shared.DTOs.Track;
using DisJockey.Shared.Helpers;

namespace DisJockey.Services.Interfaces {
    public interface ITrackRepository {
        Task<Track> GetTrackByIdAsync(int id);
        Task<Track> GetTrackByYoutubeIdAsync(string youtubeId);
        Task<PagedList<TrackListDto>> GetTracks(PaginationParams paginationParams);
        void AddTrack(Track track);
    }
}