using AutoMapper;
using DisJockey.Application.Interfaces;
using DisJockey.Services.Interfaces;
using DisJockey.Shared.DTOs.Playlist;
using ErrorOr;
using MediatR;

namespace DisJockey.Application.Features.Playlists.Commands;

public class AddPlaylistHandler : IRequestHandler<AddPlaylistCommand, ErrorOr<PlaylistDetailDto>>
{
    private readonly IPlaylistRepository _playlistRepository;
    private readonly IUserRepository _userRepository;
    private readonly IVideoDetailService _videoDetailService;
    private readonly IMapper _mapper;

    public AddPlaylistHandler(
        IPlaylistRepository playlistRepository,
        IUserRepository userRepository,
        IVideoDetailService videoDetailService,
        IMapper mapper)
    {
        _playlistRepository = playlistRepository;
        _userRepository = userRepository;
        _videoDetailService = videoDetailService;
        _mapper = mapper;
    }

    public async Task<ErrorOr<PlaylistDetailDto>> Handle(AddPlaylistCommand request, CancellationToken cancellationToken)
    {
        if (await _playlistRepository.CheckPlaylistExists(request.PlaylistId))
        {
            return Error.Conflict(description: $"Playlist with Id {request.PlaylistId} already exists");
        }

        var playlist = await _videoDetailService.GetPlaylistDetailsAsync(request.PlaylistId);

        if (playlist == null)
        {
            return Error.Validation(description: "Playlist Id Invalid");
        }

        if (playlist.Tracks.Count == 0)
            return Error.Validation(description: "No Tracks in Playlist");

        var user = await _userRepository.GetUserByDiscordIdAsync(request.DiscordId);
        if (user == null)
        {
            return Error.NotFound(description: $"User with Discord Id {request.DiscordId} not found");
        }

        await _playlistRepository.AddMissingTracks(playlist.Tracks);

        var savedPlaylist = await _playlistRepository.AddPlaylist(playlist, user);

        return _mapper.Map<PlaylistDetailDto>(savedPlaylist);
    }
}
