using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Discord;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace API.Controllers {
    public class TrackController : BaseApiController {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVideoDetailService _videoService;

        public TrackController(
            IUnitOfWork unitOfWork, 
            IVideoDetailService videoService
        ) {
            _videoService = videoService;
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        public async Task<ActionResult> AddTrack([FromBody] TrackAddDto trackDto) {
            var user = await _unitOfWork.UserRepository.GetUserByDiscordIdAsync(trackDto.DiscordId);
            if (user == null) return BadRequest("User does not exist");

            if (!trackDto.URL.Contains("youtu")) return BadRequest("Must be Youtube Link");

            var youtubeId = GetYouTubeId(trackDto.URL);
            if (youtubeId == null) return BadRequest("Something is wrong with the URL");

            var track = await _unitOfWork.TrackRepository.GetTrackByYoutubeIdAsync(youtubeId);

            if (track == null) {
                track = new Track {
                    YoutubeId = youtubeId,
                    CreatedOn = DateTime.UtcNow
                };

                try {
                    track = await _videoService.GetVideoDetails(track);
                } catch (Exception e) {
                    return BadRequest(e.ToString());
                }
            }

            var userTrack = new AppUserTrack {
                AppUserId = user.Id,
                User = user,
                TrackId = track.Id,
                Track = track
            };

            user.Tracks.Add(userTrack);

            if (await _unitOfWork.Complete()) return Ok();

            return BadRequest("Error adding track");
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MemberTrackDto>> GetTrackById(int id) {
            var track = await _unitOfWork.TrackRepository.GetTrackByIdAsync(id);

            if (track == null) return BadRequest("Track does not exist");

            MemberTrackDto trackDto = new MemberTrackDto {
                YoutubeId = track.YoutubeId,
                CreatedOn = track.CreatedOn
            };

            return Ok(trackDto);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TrackDto>>> GetTracks() {
            var tracksQuery = _unitOfWork.TrackRepository.GetTracks();

            var username = User.GetUsername();

            return Ok(await tracksQuery.ToListAsync());
        }

        [Authorize]
        [HttpPost("like")]
        public async Task<ActionResult> LikeTrack(TrackLikeDto trackLikeDto) {
            var track = await _unitOfWork.TrackRepository.GetTrackByYoutubeIdAsync(trackLikeDto.YoutubeId);

            if (track == null) return BadRequest("Track does not exist");

            var username = User.GetUsername();

            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);

            if (user == null) return Unauthorized("Invalid Token");

            var trackLike = track.Likes.FirstOrDefault(t => t.User.UserName == username);

            if (trackLike == null) {
                trackLike = new TrackLike {
                    UserId = user.Id,
                    TrackId = track.Id
                };
            } else if (trackLike.Liked == trackLikeDto.Liked) 
                return BadRequest("You already like this track");            

            trackLike.Liked = trackLikeDto.Liked;

            track.Likes.Add(trackLike);

            if (await _unitOfWork.Complete()) return Ok();

            return BadRequest("Error saving like");
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