using AutoMapper;
using DisJockey.Extensions;
using DisJockey.Services.Interfaces;
using DisJockey.Shared.DTOs.Track;
using DisJockey.Shared.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DisJockey.Controllers;

[Authorize]
public class SearchController : BaseApiController
{
    private readonly IVideoDetailService _videoDetailService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SearchController(IVideoDetailService videoDetailService, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _videoDetailService = videoDetailService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TrackListDto>>> SearchTracks([FromQuery] PaginationParams paginationParams)
    {
        var results = await _videoDetailService.QueryTracksAsync(paginationParams);

        if (results.Count == 0)
        {
            return Ok(Enumerable.Empty<TrackListDto>());
        }

        var existingTracks = await _unitOfWork.TrackRepository.GetTracksByYouTubeIdAsync(results.Select(x => x.YoutubeId));

        var resultsDto = results.Select(x => _mapper.Map<TrackListDto>(x)).ToList();

        foreach (var existingTrack in existingTracks)
        {
            var index = resultsDto.IndexOf(resultsDto.First(x => x.YoutubeId == existingTrack.YoutubeId));
            resultsDto[index] = existingTrack;
        };

        Response.AddYouTubePaginationHeader(results.CurrentPageToken, results.NextPageToken, results.PreviousPageToken);

        return Ok(resultsDto);
    }
}
