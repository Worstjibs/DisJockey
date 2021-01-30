using System.Threading.Tasks;
using API.Entities;

namespace API.Interfaces {
    public interface IVideoDetailService {
        Task<Track> GetVideoDetails(Track track);
    }
}