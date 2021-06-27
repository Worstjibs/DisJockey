using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using API.Discord.Interfaces;
using System.Collections;
using Discord.WebSocket;
using API.Discord.Services;
using Discord;
using API.Helpers;
using API.Models;

namespace API.Controllers {
    public class TracksController : BaseApiController {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVideoDetailService _videoService;
        private readonly DiscordSocketClient _client;
        private readonly MusicService _musicService;

        public TracksController(IUnitOfWork unitOfWork, IVideoDetailService videoDetailService, DiscordSocketClient client,
            MusicService musicService) {
            _musicService = musicService;
            _videoService = videoDetailService;
            _unitOfWork = unitOfWork;
            _client = client;
        }

        // [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TrackDto>>> GetTracks([FromQuery] PaginationParams paginationParams) {
            var tracks = await _unitOfWork.TrackRepository.GetTracks(paginationParams);

            var discordIdStr = User.GetDiscordId();
            ulong discordId;

            UInt64.TryParse(discordIdStr, out discordId);

            // Set Liked Flag on Tracks
            foreach (var track in tracks) {
                track.LikedByUser = track.UserLikes.FirstOrDefault(user => user.DiscordId == discordId)?.Liked; ;
            }

            Response.AddPaginationHeader(tracks.CurrentPage, tracks.ItemsPerPage, tracks.TotalPages, tracks.TotalCount);

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

        [Authorize]
        [HttpPost("play")]
        public async Task<ActionResult> PlayTrack(TrackPlayRequestDto trackPlayDto) {
            var track = await _unitOfWork.TrackRepository.GetTrackByYoutubeIdAsync(trackPlayDto.YoutubeId);

            if (track == null) return NotFound("Track with YoutubeId " + trackPlayDto.YoutubeId + "Not Found");

            ulong discordId;
            ulong.TryParse(User.GetDiscordId(), out discordId);

            var user = _client.GetUser(discordId);

            if (user != null) {
                var guild = user.MutualGuilds.FirstOrDefault();

                await _musicService.PlayTrack("https://youtu.be/" + track.YoutubeId, user, guild, trackPlayDto.PlayNow);
                return Ok();
            } else {
                return BadRequest("You must be connected to a Voice channel to play a track");
            }            
        }
    }
}