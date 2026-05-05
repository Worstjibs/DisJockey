using DisJockey.Application.Contracts;
using DisJockey.Application.Interfaces;
using ErrorOr;

namespace DisJockey.Application.Features.Tracks.Commands.BlacklistTrack;

public class BlacklistTrackHandler : IRequestHandler<BlacklistTrackCommand, ErrorOr<Success>>
{
    private readonly ITrackRepository _trackRepository;

    public BlacklistTrackHandler(ITrackRepository trackRepository)
    {
        _trackRepository = trackRepository;
    }

    public async Task<ErrorOr<Success>> HandleAsync(BlacklistTrackCommand request, CancellationToken cancellationToken)
    {
        var track = await _trackRepository.GetTrackByIdAsync(request.Id, ignoreFilters: true);
        if (track is null)
        {
            return Error.NotFound(description: $"Track with Id {request.Id} not found.");
        }

        if (track.Blacklisted)
        {
            return Error.Conflict($"Track with Id {request.Id} already blacklisted.");
        }

        track.Blacklisted = true;

        await _trackRepository.SaveChangesAsync();

        return Result.Success;
    }
}

public record BlacklistTrackCommand(int Id) : IRequest<ErrorOr<Success>>;
