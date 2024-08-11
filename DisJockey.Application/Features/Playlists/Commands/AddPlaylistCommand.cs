using DisJockey.Shared.DTOs.Playlist;
using ErrorOr;
using MediatR;

namespace DisJockey.Application.Features.Playlists.Commands;

public record AddPlaylistCommand(string PlaylistId, ulong DiscordId) : IRequest<ErrorOr<PlaylistDetailDto>>;
