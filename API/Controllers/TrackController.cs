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
        private readonly IUserRepository _userRepository;
        private readonly ITrackRepository _trackRepository;
        private readonly YouTubeService _youTubeService;

        public TrackController(IUserRepository userRepository, ITrackRepository trackRepository) {
            _youTubeService = new YouTubeService(new BaseClientService.Initializer() {
                ApiKey = "AIzaSyDBK_7dvolxDb4Jc0Am4V-DTuJPISr4Dew"
            });

            _trackRepository = trackRepository;
            _userRepository = userRepository;
        }

        [HttpPost]
        public async Task<ActionResult> AddTrack(TrackAddDto trackDto) {
            // Find the user using the UserDto in the TrackDto
            var user = await _userRepository.GetUserByDiscordIdAsync(trackDto.DiscordId);

            // If the user doesn't exist, return a BadRequest
            if (user == null) return BadRequest("User does not exist");

            // Validate the URL before attempting to get the YoutubeId from it
            if (!trackDto.URL.Contains("youtu")) return BadRequest("Must be Youtube Link");

            // Get the YoutubeId
            var youtubeId = GetYouTubeId(trackDto.URL);
            
            if (youtubeId == null) return BadRequest("Something is wrong with the URL");

            // See if the track already exists
            var track = await _trackRepository.GetTrackByYoutubeIdAsync(youtubeId);

            // If it doesn't create a record for it
            if (track == null) {
                // Query the YoutubeService for Video Details
                YoutubeVideoDto youtubeVideoDetails = await GetVideoDetails(youtubeId);

                track = new Track {
                    YoutubeId = youtubeId,
                    CreatedOn = DateTime.UtcNow,
                    Title = youtubeVideoDetails?.Title,
                    Description = youtubeVideoDetails?.Description,
                    ChannelTitle = youtubeVideoDetails?.ChannelTitle,
                    Thumbnail =  youtubeVideoDetails?.Thumbnail
                };
            }

            var userTrack = new AppUserTrack {
                AppUserId = user.Id,
                User = user,
                TrackId = track.Id,
                Track = track
            };

            // Add the track to the user
            user.Tracks.Add(userTrack);

            await _userRepository.SaveAllAsync();

            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TrackDto>> GetTrackById(int id) {
            var track = await _trackRepository.GetTrackByIdAsync(id);

            if (track == null) return BadRequest("Track does not exist");

            TrackDto trackDto = new TrackDto
            {
                YoutubeId = track.YoutubeId,
                CreatedOn = track.CreatedOn
            };

            return Ok(trackDto);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TrackUsersDto>>> GetTracks() {
            return Ok(await _trackRepository.GetTracksAsync());
        }

        [HttpPost("like")]
        public async Task<ActionResult> LikeTrack(TrackLikeDto trackLikeDto) {
            var user = await _userRepository.GetUserByUsernameAsync((trackLikeDto.Username));

            if (user == null) return BadRequest("Invalid Username");

            var track = await _trackRepository.GetTrackByYoutubeIdAsync(trackLikeDto.YoutubeId);

            if (track == null) return BadRequest("Invalid Youtube Id");

            return Ok(await _trackRepository.AddTrackLike(track, user, trackLikeDto.Liked));
        }

        private async Task<YoutubeVideoDto> GetVideoDetails(string youtubeId) {
            YoutubeVideoDto youtubeVideoDto = null;

            var searchRequest = _youTubeService.Videos.List("snippet");
            searchRequest.Id = youtubeId;

            var searchResponse = await searchRequest.ExecuteAsync();

            var youtubeVideo = searchResponse.Items.FirstOrDefault();

            if (youtubeVideo != null) {
                youtubeVideoDto = new YoutubeVideoDto {
                    Title = youtubeVideo.Snippet.Title,
                    Description = youtubeVideo.Snippet.Description,
                    ChannelTitle = youtubeVideo.Snippet.ChannelTitle,
                    Thumbnail = youtubeVideo.Snippet.Thumbnails.Medium.Url
                };
            }

            return youtubeVideoDto;
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