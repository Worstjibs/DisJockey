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
                .Include(x => x.AppUsers)
                .FirstOrDefaultAsync(x => x.YoutubeId == youtubeId);
        }

        public async Task<IEnumerable<TrackUsersDto>> GetTracksAsync() {
            var tracks = _context.Tracks.Include(x => x.AppUsers);

            return await _context.Tracks
                .Include(x => x.AppUsers)
                .ProjectTo<TrackUsersDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }
    }
}