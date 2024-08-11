using AutoMapper;
using DisJockey.Application.Interfaces;
using DisJockey.Services.Interfaces;
using DisJockey.Shared.DTOs.Track;
using MediatR;
using static DisJockey.Application.Features.Search.Queries.SearchHandler;

namespace DisJockey.Application.Features.Search.Queries;

public class SearchHandler : IRequestHandler<SearchQuery, (IEnumerable<TrackListDto> Results, YouTubePagination? Pagination)>
{
    private readonly IVideoDetailService _videoDetailService;
    private readonly ITrackRepository _trackRepository;
    private readonly IMapper _mapper;

    public SearchHandler(
        IVideoDetailService videoDetailService,
        ITrackRepository trackRepository,
        IMapper mapper)
    {
        _videoDetailService = videoDetailService;
        _trackRepository = trackRepository;
        _mapper = mapper;
    }

    public async Task<(IEnumerable<TrackListDto> Results, YouTubePagination? Pagination)> Handle(SearchQuery request, CancellationToken cancellationToken)
    {
        var results = await _videoDetailService.QueryTracksAsync(request.Pagination);
        if (results.Count == 0)
        {
            return ([], null);
        }

        var existingTracks = await _trackRepository.GetTracksByYouTubeIdAsync(results.Select(x => x.YoutubeId));

        var resultsDto = results.Select(_mapper.Map<TrackListDto>).ToList();

        foreach (var existingTrack in existingTracks)
        {
            var index = resultsDto.IndexOf(resultsDto.First(x => x.YoutubeId == existingTrack.YoutubeId));
            resultsDto[index] = existingTrack;
        };

        var youTubePagination = new YouTubePagination(
            results.CurrentPageToken,
            results.NextPageToken,
            results.PreviousPageToken);

        return (resultsDto, youTubePagination);
    }

    public class YouTubePagination
    {
        public string CurrentPageToken { get; set; }
        public string NextPageToken { get; set; }
        public string PreviousPageToken { get; set; }

        public YouTubePagination(string currentPageToken, string nextPageToken, string previousPageToken)
        {
            CurrentPageToken = currentPageToken;
            NextPageToken = nextPageToken;
            PreviousPageToken = previousPageToken;
        }
    }
}
