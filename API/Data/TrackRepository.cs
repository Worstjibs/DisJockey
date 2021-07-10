using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using API.Helpers;
using API.DTOs.Track;

namespace API.Data {
    public class TrackRepository : ITrackRepository {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public TrackRepository(DataContext context, IMapper mapper) {
            _mapper = mapper;
            _context = context;
        }

        public async Task<Track> GetTrackByIdAsync(int id) {
            return await _context.Tracks.FindAsync(id);
        }

        public async Task<Track> GetTrackByYoutubeIdAsync(string youtubeId) {
            return await _context.Tracks
                .Include(x => x.Likes).ThenInclude(x => x.User)
                .Include(x => x.TrackPlays).ThenInclude(x => x.TrackPlayHistory)
                .Include(x => x.PullUps)
                .FirstOrDefaultAsync(x => x.YoutubeId == youtubeId);
        }

        public async Task<PagedList<TrackDto>> GetTracks(PaginationParams paginationParams) {
            var userTracks = _context.Tracks.AsQueryable().AsNoTracking()
                .Where(x => x.TrackPlays.Count > 0)
                .ProjectTo<TrackDto>(_mapper.ConfigurationProvider);

            switch (paginationParams.SortBy) {
                case "title":
                    userTracks = userTracks.OrderBy(x => x.Title);
                    break;
                case "firstPlayed":
                    userTracks = userTracks.OrderBy(x => x.LastPlayed);
                    break;
                default:
                    userTracks = userTracks.OrderByDescending(x => x.LastPlayed);
                    break;
            }

            return await PagedList<TrackDto>.CreateAsync(userTracks, paginationParams.PageNumber, paginationParams.PageSize);
        }

        public void AddTrack(Track track) {
            _context.Tracks.Add(track);
        }
    }
}