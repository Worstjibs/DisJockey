using DisJockey.Extensions;
using DisJockey.Services.Interfaces;
using DisJockey.Shared.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DisJockey.Controllers {
    public class SearchController : BaseApiController {
        private readonly IVideoDetailService _videoDetailService;
        private readonly IUnitOfWork _unitOfWork;

        public SearchController(IVideoDetailService videoDetailService, IUnitOfWork unitOfWork) {
            _videoDetailService = videoDetailService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult<YouTubePagedList<string>>> SearchTracks([FromQuery] PaginationParams paginationParams) {
            var results = await _videoDetailService.QueryTracksAsync(paginationParams);

            if (results == null) {
                return NoContent();
            }

            var existingTracks = await _unitOfWork.TrackRepository.GetTracksByYouTubeIdAsync(results.Select(x => x.YoutubeId));

            foreach (var existingTrack in existingTracks) {
                var index = results.IndexOf(results.First(x => x.YoutubeId == existingTrack.YoutubeId));
                results[index] = existingTrack;
            };

            Response.AddYouTubePaginationHeader(results.CurrentPageToken, results.NextPageToken, results.PreviousPageToken);

            return Ok(results);
        }
    }
}
