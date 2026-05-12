using DisJockey.Application.Contracts;
using DisJockey.Application.Interfaces;
using DisJockey.Shared.Messaging.Contracts;
using DisJockey.Shared.Messaging.Events;
using ErrorOr;

namespace DisJockey.Application.Features.Tracks.Commands.PlayTrack;

public class PlayTrackHandler : IRequestHandler<PlayTrackCommand, ErrorOr<Success>>
{
    private readonly ITrackRepository _trackRepository;
    private readonly IMessageSender _sender;

    public PlayTrackHandler(
        ITrackRepository trackRepository,
        IMessageSender sender)
    {
        _trackRepository = trackRepository;
        _sender = sender;
    }

    public async Task<ErrorOr<Success>> HandleAsync(PlayTrackCommand request, CancellationToken cancellationToken)
    {
        var trackIsBlacklisted = await _trackRepository.IsTrackBlacklisted(request.YouTubeId);
        if (trackIsBlacklisted)
        {
            return Error.Validation(description: "Track is blacklisted");
        }

        var playTrackEvent = new PlayTrackEvent
        {
            DiscordId = request.DiscordId,
            YoutubeId = request.YouTubeId,
            Queue = !request.PlayNow
        };

        await _sender.SendAsync(playTrackEvent);

        return Result.Success;
    }
}

public record PlayTrackCommand(string YouTubeId, ulong DiscordId, bool PlayNow) : IRequest<ErrorOr<Success>>;
