using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;

namespace API.Interfaces {
    public interface ITrackRepository {
        Task<IEnumerable<TrackDto>> GetTracksAsync();
        Task<Track> GetTrackByIdAsync(int id);
    }
}