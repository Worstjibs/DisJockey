using DisJockey.Application.Contracts;
using DisJockey.Application.Interfaces;
using DisJockey.Application.Mappers;
using DisJockey.Services.Interfaces;
using DisJockey.Shared.DTOs.Playlist;
using ErrorOr;

namespace DisJockey.Application.Features.Playlists.Commands;

public class AddPlaylistHandler : IRequestHandler<AddPlaylistCommand, ErrorOr<PlaylistDetailDto>>
{
    private readonly IPlaylistRepository _playlistRepository;
    private readonly IUserRepository _userRepository;
    private readonly IVideoDetailService _videoDetailService;

    public AddPlaylistHandler(
        IPlaylistRepository playlistRepository,
        IUserRepository userRepository,
        IVideoDetailService videoDetailService)
    {
        _playlistRepository = playlistRepository;
        _userRepository = userRepository;
        _videoDetailService = videoDetailService;
    }

    public async Task<ErrorOr<PlaylistDetailDto>> HandleAsync(
        AddPlaylistCommand request,
        CancellationToken cancellationToken = default)
    {
        var playlistExists = await _playlistRepository.CheckPlaylistExists(request.PlaylistId);
        if (playlistExists)
        {
            return Error.Conflict(description: $"Playlist with Id {request.PlaylistId} already exists");
        }

        var playlist = await _videoDetailService.GetPlaylistDetailsAsync(request.PlaylistId);
        if (playlist is null)
        {
            return Error.Validation(description: "Playlist Id Invalid");
        }

        if (playlist.Tracks.Count == 0)
        {
            return Error.Validation(description: "No Tracks in Playlist");
        }

        var user = await _userRepository.GetUserByDiscordIdAsync(request.DiscordId);
        if (user is null)
        {
            return Error.NotFound(description: $"User with Discord Id {request.DiscordId} not found");
        }

        await _playlistRepository.AddMissingTracks(playlist.Tracks);

        var savedPlaylist = await _playlistRepository.AddPlaylist(playlist, user);

        return savedPlaylist.ToDetailDto();
    }
}

public record AddPlaylistCommand(string PlaylistId, ulong DiscordId) : IRequest<ErrorOr<PlaylistDetailDto>>;
