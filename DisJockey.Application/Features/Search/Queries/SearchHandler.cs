using DisJockey.Application.Contracts;
using DisJockey.Application.Interfaces;
using DisJockey.Application.Mappers;
using DisJockey.Shared.DTOs.Track;
using DisJockey.Shared.Helpers;

namespace DisJockey.Application.Features.Search.Queries;

public class SearchHandler : IRequestHandler<SearchQuery, (IEnumerable<TrackListDto> Results, YouTubePagination? Pagination)>
{
    private readonly IVideoDetailService _videoDetailService;
    private readonly ITrackRepository _trackRepository;

    public SearchHandler(
        IVideoDetailService videoDetailService,
        ITrackRepository trackRepository)
    {
        _videoDetailService = videoDetailService;
        _trackRepository = trackRepository;
    }

    public async Task<(IEnumerable<TrackListDto> Results, YouTubePagination? Pagination)> HandleAsync(
        SearchQuery request, CancellationToken cancellationToken = default)
    {
        var results = await _videoDetailService.QueryTracksAsync(request.Pagination);
        if (results.Count == 0)
            return ([], null);

        var existingTracks = await _trackRepository.GetTracksByYouTubeIdAsync(results.Select(x => x.YoutubeId));

        var resultsDto = results.Select(t => t.ToListDto()).ToList();

        foreach (var existingTrack in existingTracks)
        {
            var index = resultsDto.IndexOf(resultsDto.First(x => x.YoutubeId == existingTrack.YoutubeId));
            resultsDto[index] = existingTrack;
        }

        var pagination = new YouTubePagination(results.CurrentPageToken, results.NextPageToken, results.PreviousPageToken);
        return (resultsDto, pagination);
    }
}

public record SearchQuery(PaginationParams Pagination)
    : IRequest<(IEnumerable<TrackListDto> Results, YouTubePagination? Pagination)>;
