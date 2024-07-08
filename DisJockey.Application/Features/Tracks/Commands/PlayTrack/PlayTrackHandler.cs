using DisJockey.MassTransit.Events;
using DisJockey.Services.Interfaces;
using ErrorOr;
using MassTransit;
using MediatR;

namespace DisJockey.Application.Features.Tracks.Commands.PlayTrack;

public class PlayTrackHandler : IRequestHandler<PlayTrackCommand, ErrorOr<Success>>
{
    private readonly ITrackRepository _trackRepository;
    private readonly IBus _bus;

    public PlayTrackHandler(ITrackRepository trackRepository, IBus bus)
    {
        _trackRepository = trackRepository;
        _bus = bus;
    }

    public async Task<ErrorOr<Success>> Handle(PlayTrackCommand request, CancellationToken cancellationToken)
    {
        if (await _trackRepository.IsTrackBlacklisted(request.YouTubeId))
            return Error.Failure(description: "Track is blacklisted");

        var playTrackEvent = new PlayTrackEvent
        {
            DiscordId = request.DiscordId,
            YoutubeId = request.YouTubeId,
            Queue = !request.PlayNow
        };

        await _bus.Publish(playTrackEvent);

        return Result.Success;
    }
}
