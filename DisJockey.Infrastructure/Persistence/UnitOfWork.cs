using AutoMapper;
using DisJockey.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace DisJockey.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContext;
    private readonly IUserRepository _userRepository;
    private readonly ITrackRepository _trackRepository;
    private readonly IPlaylistRepository _playlistRepository;

    public UnitOfWork(
        DataContext context,
        IMapper mapper,
        IHttpContextAccessor httpContext,
        IUserRepository userRepository,
        ITrackRepository trackRepository,
        IPlaylistRepository playlistRepository)
    {
        _mapper = mapper;
        _httpContext = httpContext;
        _userRepository = userRepository;
        _trackRepository = trackRepository;
        _playlistRepository = playlistRepository;
        _context = context;
    }

    public IUserRepository UserRepository => _userRepository;
    public ITrackRepository TrackRepository => _trackRepository;
    public IPlaylistRepository PlaylistRepository => _playlistRepository;

    public async Task<bool> Complete()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    public bool HasChanges()
    {
        return _context.ChangeTracker.HasChanges();
    }
}