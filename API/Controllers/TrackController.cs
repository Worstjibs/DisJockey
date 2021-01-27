using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace API.Controllers {
    public class TrackController : BaseApiController {
        private readonly YouTubeService _youTubeService;
        private readonly IUnitOfWork _unitOfWork;

        public TrackController(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
            _youTubeService = new YouTubeService(new BaseClientService.Initializer() {
                ApiKey = "AIzaSyDBK_7dvolxDb4Jc0Am4V-DTuJPISr4Dew"
            });
        }

        [HttpPost]
        public async Task<ActionResult> AddTrack(TrackAddDto trackDto) {
            // Find the user using the UserDto in the TrackDto
            var user = await _unitOfWork._userRepository.GetUserByDiscordIdAsync(trackDto.DiscordId);

            // If the user doesn't exist, return a BadRequest
            if (user == null) return BadRequest("User does not exist");

            // Validate the URL before attempting to get the YoutubeId from it
            if (!trackDto.URL.Contains("youtu")) return BadRequest("Must be Youtube Link");

            // Get the YoutubeId
            var youtubeId = GetYouTubeId(trackDto.URL);

            if (youtubeId == null) return BadRequest("Something is wrong with the URL");

            // See if the track already exists
            var track = await _unitOfWork._trackRepository.GetTrackByYoutubeIdAsync(youtubeId);

            // If it doesn't create a record for it
            if (track == null) {
                track = new Track {
                    YoutubeId = youtubeId,
                    CreatedOn = DateTime.UtcNow
                };

                try {
                    await GetVideoDetails(track);
                } catch (Exception e) {
                    return BadRequest(e.ToString());
                }
            } else {
                // Check if the user has already posted the track
                if (user.Tracks.FirstOrDefault(ut => ut.TrackId == track.Id) != null) {
                    return BadRequest("User has already posted this track");
                }
            }

            var userTrack = new AppUserTrack {
                AppUserId = user.Id,
                User = user,
                TrackId = track.Id,
                Track = track
            };

            // Add the track to the user
            user.Tracks.Add(userTrack);

            await _unitOfWork.Complete();

            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TrackDto>> GetTrackById(int id) {
            var track = await _unitOfWork._trackRepository.GetTrackByIdAsync(id);

            if (track == null) return BadRequest("Track does not exist");

            TrackDto trackDto = new TrackDto {
                YoutubeId = track.YoutubeId,
                CreatedOn = track.CreatedOn
            };

            return Ok(trackDto);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TrackUsersDto>>> GetTracks() {
            return Ok(await _unitOfWork._trackRepository.GetTracksAsync());
        }

        private async Task GetVideoDetails(Track track) {
            var searchRequest = _youTubeService.Videos.List("snippet");
            searchRequest.Id = track.YoutubeId;

            var searchResponse = await searchRequest.ExecuteAsync();

            var youtubeVideo = searchResponse.Items.FirstOrDefault();

            if (youtubeVideo != null) {
                track.Title = youtubeVideo.Snippet.Title;
                track.Description = youtubeVideo.Snippet.Description;
                track.ChannelTitle = youtubeVideo.Snippet.ChannelTitle;
                track.SmallThumbnail = youtubeVideo.Snippet.Thumbnails.Medium.Url;
                track.MediumThumbnail = youtubeVideo.Snippet.Thumbnails.High.Url;
                track.LargeThumbnail = youtubeVideo.Snippet.Thumbnails.Standard.Url;
            }

            throw new Exception("Invalid Youtube Id");
        }

        public string GetYouTubeId(string url) {
            try {
                Uri uri = new Uri(url);

                if (uri == null) return null;

                string youtubeId = null;

                if (url.Contains("youtube.com")) {
                    var queryString = QueryHelpers.ParseQuery(uri.Query);

                    if (queryString.ContainsKey("v")) youtubeId = queryString["v"];
                } else if (url.Contains("youtu.be")) {
                    youtubeId = uri.Segments[1];
                }

                return youtubeId;
            } catch {
                throw new Exception("Something went wrong");
            }
        }
    }
}