using DisJockey.Shared.Messaging.Events;
using DisJockey.Services.Interfaces;
using DisJockey.Shared.Messaging.Contracts;
using ErrorOr;
using MediatR;

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

    public async Task<ErrorOr<Success>> Handle(PlayTrackCommand request, CancellationToken cancellationToken)
    {
        if (await _trackRepository.IsTrackBlacklisted(request.YouTubeId))
            return Error.Validation(description: "Track is blacklisted");

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
