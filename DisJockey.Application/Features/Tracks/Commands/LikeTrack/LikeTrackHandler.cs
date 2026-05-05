using DisJockey.Application.Contracts;
using DisJockey.Application.Interfaces;
using DisJockey.Core;
using DisJockey.Services.Interfaces;
using DisJockey.Shared.Exceptions;
using ErrorOr;

namespace DisJockey.Application.Features.Tracks.Commands.LikeTrack;

public class LikeTrackHandler : IRequestHandler<LikeTrackCommand, ErrorOr<Success>>
{
    private readonly ITrackRepository _trackRepository;
    private readonly IUserRepository _userRepository;

    public LikeTrackHandler(ITrackRepository trackRepository, IUserRepository userRepository)
    {
        _trackRepository = trackRepository;
        _userRepository = userRepository;
    }

    public async Task<ErrorOr<Success>> HandleAsync(LikeTrackCommand request, CancellationToken cancellationToken)
    {
        var track = await _trackRepository.GetTrackByYoutubeIdAsync(request.YouTubeId);
        if (track is null)
        {
            return Error.NotFound(description: "Track not found");
        }

        var user = await _userRepository.GetUserByDiscordIdAsync(request.DiscordId)
            ?? throw new UserNotFoundException($"User with discord Id {request.DiscordId} not found");

        var trackLike = track.Likes.FirstOrDefault(t => t.User.DiscordId == request.DiscordId);

        if (trackLike == null)
        {
            trackLike = new TrackLike
            {
                UserId = user.Id,
                TrackId = track.Id
            };
        }
        else if (trackLike.Liked == request.Liked)
        {
            return Error.Conflict(description: "You already like this track");
        }

        trackLike.Liked = request.Liked;

        track.Likes.Add(trackLike);

        var saveResult = await _trackRepository.SaveChangesAsync();
        if (!saveResult)
        {
            return Error.Failure(description: "Something went wrong saving the TrackLike");
        }

        return Result.Success;
    }
}

public record LikeTrackCommand(string YouTubeId, ulong DiscordId, bool Liked) : Contracts.IRequest<ErrorOr<Success>>;
