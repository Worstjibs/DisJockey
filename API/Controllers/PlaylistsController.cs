using System.Threading.Tasks;
using API.DTOs.Playlist;
using API.Interfaces;
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
        public async Task<ActionResult> AddPlayList(PlaylistAddDto playlistDto) {
            if (await _unitOfWork.PlaylistRepository.CheckPlaylistExists(playlistDto.PlaylistId)) {
                return Conflict($"Playlist with Id {playlistDto.PlaylistId} already exists");
            }

            var playlist = await _videoDetailService.GetPlaylistDetails(playlistDto.PlaylistId);

            if (playlist == null) {
                return NotFound("Playlist Id Invalid");
            }
            if (playlist.Tracks.Count == 0) return BadRequest("No Tracks in Playlist");

            var user = await _unitOfWork.UserRepository.GetUserByDiscordIdAsync(playlistDto.DiscordId);
            if (user == null) {
                return NotFound($"User with Discord Id {playlistDto.DiscordId} not found");
            }

            await _unitOfWork.PlaylistRepository.AddMissingTracks(playlist.Tracks);

            return Ok(await _unitOfWork.PlaylistRepository.AddPlaylist(playlist, user));
        }

        [HttpGet("{youtubeId}")]
        public async Task<ActionResult<PlaylistDto>> GetPlaylist(string youtubeId) {
            return await _unitOfWork.PlaylistRepository.GetPlaylistByYoutubeId(youtubeId);
        }
    }
}