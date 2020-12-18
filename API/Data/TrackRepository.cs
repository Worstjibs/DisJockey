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
            var tracks = _context.Tracks;

            return await tracks.FindAsync(id);
        }

        public async Task<IEnumerable<TrackDto>> GetTracksAsync() {
            return await _context.Tracks
                .ProjectTo<TrackDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }
    }
}