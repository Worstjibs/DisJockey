using ErrorOr;
using MediatR;

namespace DisJockey.Application.Features.Tracks.Commands.PlayTrack;

public record PlayTrackCommand(string YouTubeId, ulong DiscordId, bool PlayNow) : IRequest<ErrorOr<Success>>;
