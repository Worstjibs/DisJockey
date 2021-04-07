using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Models;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using API.Discord.Interfaces;
using System.Collections;

namespace API.Controllers {
    public class TrackController : BaseApiController {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVideoDetailService _videoService;
        private readonly IDiscordTrackService _discordTrackService;

        public TrackController(
            IUnitOfWork unitOfWork, 
            IVideoDetailService videoService,
            IDiscordTrackService discordTrackService
        ) {
            _videoService = videoService;
            _unitOfWork = unitOfWork;
            _discordTrackService = discordTrackService;
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

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable>> GetTracks() {
            var tracksQuery = _unitOfWork.TrackRepository.GetTracks();

            var tracks = await tracksQuery.ToListAsync();

            foreach(var track in tracks) {
                track.Users = (from user in track.Users
                    group user by new { user.DiscordId, user.Username } into grouping
                    select new TrackUserDto {
                        DiscordId = grouping.Key.DiscordId,
                        Username = grouping.Key.Username,
                        TimesPlayed = grouping.Count(),
                        CreatedOn = grouping.Min(x => x.CreatedOn),
                        LastPlayed = grouping.Max(x => x.CreatedOn)
                    }).ToList();
            }

            return Ok(tracks);
        }

        [Authorize]
        [HttpPost("like")]
        public async Task<ActionResult> LikeTrack(TrackLikeAddDto trackLikeDto) {
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

        [HttpPost]
        public async Task<ActionResult> AddTrack(TrackAddDto trackAddDto) {
            try {
                return Ok(await _discordTrackService.AddTrackAsync(trackAddDto.DiscordId, trackAddDto.Username, trackAddDto.URL));
            } catch (Exception e) {
                return BadRequest(e);
            }
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