using DisJockey.Services.Interfaces;
using DisJockey.Shared.DTOs.Track;
using DisJockey.Shared.Helpers;
using MediatR;

namespace DisJockey.Application.Features.Tracks.Queries.TracksForMember;

public class TracksForMemberHandler : IRequestHandler<TracksForMemberQuery, PagedList<TrackListDto>>
{
    private readonly ITrackRepository _trackRepository;

    public TracksForMemberHandler(ITrackRepository trackRepository)
    {
        _trackRepository = trackRepository;
    }

    public async Task<PagedList<TrackListDto>> Handle(TracksForMemberQuery request, CancellationToken cancellationToken)
    {
        var tracks = await _trackRepository.GetTrackPlaysForMember(request.Pagination, request.DiscordId);

        return tracks;
    }
}
