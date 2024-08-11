using DisJockey.Core;
using DisJockey.Services.Interfaces;
using ErrorOr;
using MediatR;

namespace DisJockey.Application.Features.Tracks.Commands.BlacklistTrack;

public class BlacklistTrackHandler : IRequestHandler<BlacklistTrackCommand, ErrorOr<Success>>
{
    private readonly ITrackRepository _trackRepository;

    public BlacklistTrackHandler(ITrackRepository trackRepository)
    {
        _trackRepository = trackRepository;
    }

    public async Task<ErrorOr<Success>> Handle(BlacklistTrackCommand request, CancellationToken cancellationToken)
    {
        var track = await _trackRepository.GetTrackByIdAsync(request.Id);

        if (track == null)
            return Error.NotFound(description: $"Track with Id {request.Id} not found.");

        if (track.Blacklisted)
            return Error.Conflict($"Track with Id {request.Id} already blacklisted.");

        track.Blacklisted = true;

        await _trackRepository.SaveChangesAsync();

        return Result.Success;
    }
}
