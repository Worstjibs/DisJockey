using System.Threading.Tasks;
using API.DTOs;
using API.Interfaces;
using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers {
    public class PlaylistsController : BaseApiController {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVideoDetailService _videoDetailService;
        public PlaylistsController(IUnitOfWork unitOfWork, IVideoDetailService videoDetailService) {
            _videoDetailService = videoDetailService;
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        public async Task<ActionResult> AddPlayList(PlaylistDto playlistDto) {
            var playlist = await _videoDetailService.GetPlaylistDetails(playlistDto.PlaylistId);

            if (playlist == null) return NotFound("Playlist Id Invalid");

            if (playlist.Tracks.Count == 0) return BadRequest("No Tracks in Playlist");

            await _unitOfWork.TrackRepository.AddMissingTracks(playlist.Tracks);

            await _unitOfWork.TrackRepository.AddPlaylist(playlist);

            await _unitOfWork.Complete();

            return Ok();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PlaylistModel>> GetPlaylist(int playlistId) {
            return await _unitOfWork.TrackRepository.GetPlaylist(playlistId);
        }
    }
}