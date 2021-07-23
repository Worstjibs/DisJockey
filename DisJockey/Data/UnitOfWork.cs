using System.Threading.Tasks;
using AutoMapper;
using DisJockey.Services.Interfaces;

namespace DisJockey.Data {
    public class UnitOfWork : IUnitOfWork {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public UnitOfWork(DataContext context, IMapper mapper) {
            _mapper = mapper;
            _context = context;
        }

        public IUserRepository UserRepository => new UserRepository(_context, _mapper);
        public ITrackRepository TrackRepository => new TrackRepository(_context, _mapper);

        public IPlaylistRepository PlaylistRepository => new PlaylistRepository(_context, _mapper);

        public async Task<bool> Complete() {
            return await _context.SaveChangesAsync() > 0;
        }

        public bool HasChanges() {
            return _context.ChangeTracker.HasChanges();
        }
    }
}