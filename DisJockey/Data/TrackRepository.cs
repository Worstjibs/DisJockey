using System.Linq;
using System.Threading.Tasks;
using DisJockey.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using DisJockey.Shared.DTOs.Track;
using DisJockey.Shared.Helpers;
using DisJockey.Services.Interfaces;
using DisJockey.Shared.DTOs.Shared;

namespace DisJockey.Data {
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

        public async Task<PagedList<TrackListDto>> GetTracks(PaginationParams paginationParams) {
            var query = _context.Tracks.AsNoTracking()
                .Where(x => x.TrackPlays.Count > 0)
                .ProjectTo<TrackListDto>(_mapper.ConfigurationProvider);

            return await CreatePagedList(paginationParams, query);
        }

        public async Task<PagedList<TrackListDto>> GetTrackPlaysForMember(PaginationParams paginationParams, ulong discordId) {
            var query = _context.Tracks.AsNoTracking()
                .Where(x => x.TrackPlays.Any(tp => tp.User.DiscordId == discordId))
                .ProjectTo<TrackListDto>(_mapper.ConfigurationProvider);

            return await CreatePagedList(paginationParams, query);
        }

        public void AddTrack(Track track) {
            _context.Tracks.Add(track);
        }

        private static async Task<PagedList<TrackListDto>> CreatePagedList(PaginationParams paginationParams, IQueryable<TrackListDto> source) {
            source = paginationParams.SortBy switch {
                "title" => source.OrderBy(x => x.Title),
                "firstPlayed" => source.OrderBy(x => x.LastPlayed),
                _ => source.OrderByDescending(x => x.LastPlayed),
            };

            return await PagedList<TrackListDto>.CreateAsync(source, paginationParams.PageNumber, paginationParams.PageSize);
        }
    }
}