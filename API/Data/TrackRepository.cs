using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

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
                .FirstOrDefaultAsync(x => x.YoutubeId == youtubeId);
        }

        public IQueryable<TrackDto> GetTracks() {
            var userTracks = _context.Tracks
                .Include(t => t.AppUsers).ThenInclude(ut => ut.User)
                .Include(t => t.Likes).ThenInclude(tl => tl.User)
                .ProjectTo<TrackDto>(_mapper.ConfigurationProvider)
                .OrderByDescending(t => t.CreatedOn);

            return userTracks;
        }
    }
}