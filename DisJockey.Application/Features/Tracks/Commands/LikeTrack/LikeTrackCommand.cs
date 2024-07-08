using ErrorOr;
using MediatR;

namespace DisJockey.Application.Features.Tracks.Commands.LikeTrack;

public record LikeTrackCommand(string YouTubeId, ulong DiscordId, bool Liked) : IRequest<ErrorOr<Success>>;
