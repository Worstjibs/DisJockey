using DisJockey.Application.Contracts;
using DisJockey.Application.Interfaces;
using DisJockey.Shared.DTOs.Track;
using DisJockey.Shared.Helpers;

namespace DisJockey.Application.Features.Tracks.Queries.AllTracks;

public class AllTracksHandler : IRequestHandler<AllTracksQuery, PagedList<TrackListDto>>
{
    private readonly ITrackRepository _trackRepository;

    public AllTracksHandler(ITrackRepository trackRepository)
    {
        _trackRepository = trackRepository;
    }

    public async Task<PagedList<TrackListDto>> HandleAsync(AllTracksQuery request, CancellationToken cancellationToken)
    {
        var tracks = await _trackRepository.GetTracks(request.Pagination);

        return tracks;
    }
}

public record AllTracksQuery(PaginationParams Pagination) : IRequest<PagedList<TrackListDto>>;
