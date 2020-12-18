using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers {
    public class TrackController : BaseApiController {
        private readonly IUserRepository _userRepository;
        private readonly ITrackRepository _trackRepository;

        public TrackController(IUserRepository userRepository, ITrackRepository trackRepository) {
            _trackRepository = trackRepository;
            _userRepository = userRepository;
        }

        [HttpPost]
        public async Task<ActionResult> AddTrack(TrackAddDto trackDto) {
            // Find the user using the UserDto in the TrackDto
            var user = await _userRepository.GetUserByUsernameAsync(trackDto.user.Username);

            // If the user doesn't exist, return a BadRequest
            if (user == null) return BadRequest("User does not exist");

            if (!trackDto.URL.Contains("https://youtu")) return BadRequest("Must be Youtube Link");

            var track = new Track
            {
                URL = trackDto.URL,
                CreatedOn = DateTime.Now
            };

            user.Tracks.Add(track);

            await _userRepository.SaveAllAsync();

            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TrackDto>> GetTrackById(int id) {
            var track = await _trackRepository.GetTrackByIdAsync(id);

            if (track == null) return BadRequest("Track does not exist");

            TrackDto trackDto = new TrackDto {
                URL = track.URL,
                CreatedOn = track.CreatedOn
            };

            return Ok(trackDto);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TrackDto>>> GetTracks() {
            return Ok(await _trackRepository.GetTracksAsync());
        }
    }
}