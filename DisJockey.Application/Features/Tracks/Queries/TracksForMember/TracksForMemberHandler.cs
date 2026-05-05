using DisJockey.Application.Contracts;
using DisJockey.Application.Interfaces;
using DisJockey.Shared.DTOs.Track;
using DisJockey.Shared.Helpers;

namespace DisJockey.Application.Features.Tracks.Queries.TracksForMember;

public class TracksForMemberHandler : IRequestHandler<TracksForMemberQuery, PagedList<TrackListDto>>
{
    private readonly ITrackRepository _trackRepository;

    public TracksForMemberHandler(ITrackRepository trackRepository)
    {
        _trackRepository = trackRepository;
    }

    public async Task<PagedList<TrackListDto>> HandleAsync(TracksForMemberQuery request, CancellationToken cancellationToken)
    {
        var tracks = await _trackRepository.GetTrackPlaysForMember(request.Pagination, request.DiscordId);

        return tracks;
    }
}

public record TracksForMemberQuery(PaginationParams Pagination, ulong DiscordId) : IRequest<PagedList<TrackListDto>>;
