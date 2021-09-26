using System.Threading.Tasks;
using DisJockey.Shared.DTOs.Playlist;
using DisJockey.Extensions;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using DisJockey.Services.Interfaces;
using DisJockey.Shared.Helpers;
using Microsoft.AspNetCore.Authorization;
using DisJockey.Shared.Extensions;
using DisJockey.Shared.DTOs.Track;

namespace DisJockey.Controllers {
    [Authorize]
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

            var playlist = await _videoDetailService.GetPlaylistDetailsAsync(playlistDto.PlaylistId);

            if (playlist == null) {
                return BadRequest("Playlist Id Invalid");
            }
            if (playlist.Tracks.Count == 0) return BadRequest("No Tracks in Playlist");

            var discordId = User.GetDiscordId();
            if (!discordId.HasValue) {
                return BadRequest("Invalid DiscordId");
            }

            var user = await _unitOfWork.UserRepository.GetUserByDiscordIdAsync(discordId.Value);
            if (user == null) {
                return NotFound($"User with Discord Id {discordId} not found");
            }

            await _unitOfWork.PlaylistRepository.AddMissingTracks(playlist.Tracks);

            var savedPlaylist = await _unitOfWork.PlaylistRepository.AddPlaylist(playlist, user);

            return Ok(_mapper.Map<PlaylistDetailDto>(savedPlaylist));
        }

        [HttpGet("{youtubeId}")]
        public async Task<ActionResult<PagedList<TrackListDto>>> GetPlaylistTracks([FromQuery] PaginationParams paginationParams, string youtubeId) {
            var playlistTracks = await _unitOfWork.PlaylistRepository.GetPlaylistTracks(paginationParams, youtubeId);

            Response.AddPaginationHeader(playlistTracks.CurrentPage, playlistTracks.ItemsPerPage, playlistTracks.TotalPages, playlistTracks.TotalCount);

            return playlistTracks;
        }
    }
}