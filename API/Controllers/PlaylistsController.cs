using System.Threading.Tasks;
using API.DTOs.Playlist;
using API.DTOs.Shared;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers {
    public class PlaylistsController : BaseApiController {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVideoDetailService _videoDetailService;
        private readonly IMapper _mapper;

        public PlaylistsController(IUnitOfWork unitOfWork, IVideoDetailService videoDetailService, IMapper mapper) {
            _videoDetailService = videoDetailService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        public async Task<ActionResult<PlaylistDetailDto>> AddPlayList(PlaylistAddDto playlistDto) {
            if (await _unitOfWork.PlaylistRepository.CheckPlaylistExists(playlistDto.PlaylistId)) {
                return Conflict($"Playlist with Id {playlistDto.PlaylistId} already exists");
            }

            var playlist = await _videoDetailService.GetPlaylistDetails(playlistDto.PlaylistId);

            if (playlist == null) {
                return BadRequest("Playlist Id Invalid");
            }
            if (playlist.Tracks.Count == 0) return BadRequest("No Tracks in Playlist");

            ulong.TryParse(User.GetDiscordId(), out ulong discordId);

            var user = await _unitOfWork.UserRepository.GetUserByDiscordIdAsync(discordId);
            if (user == null) {
                return NotFound($"User with Discord Id {discordId} not found");
            }

            await _unitOfWork.PlaylistRepository.AddMissingTracks(playlist.Tracks);

            var savedPlaylist = await _unitOfWork.PlaylistRepository.AddPlaylist(playlist, user);

            return Ok(_mapper.Map<PlaylistDetailDto>(savedPlaylist));
        }

        [HttpGet("{youtubeId}")]
        public async Task<ActionResult<PlaylistDetailDto>> GetPlaylist(string youtubeId) {
            return await _unitOfWork.PlaylistRepository.GetPlaylistByYoutubeId(youtubeId);
        }
    }
}