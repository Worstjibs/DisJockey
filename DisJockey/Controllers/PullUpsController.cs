using DisJockey.Extensions;
using DisJockey.Services.Interfaces;
using DisJockey.Shared.DTOs.PullUps;
using DisJockey.Shared.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DisJockey.Controllers {
    public class PullUpsController : BaseApiController {
        private readonly IUnitOfWork _unitOfWork;

        public PullUpsController(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("{discordId}")]
        public async Task<ActionResult<PagedList<PullUpDto>>> GetPullUpsForMember([FromQuery] PaginationParams paginationParams, ulong discordId) {
            var tracks = await _unitOfWork.TrackRepository.GetPullUpsForMember(paginationParams, discordId);

            Response.AddPaginationHeader(tracks.CurrentPage, tracks.ItemsPerPage, tracks.TotalPages, tracks.TotalCount);

            return Ok(tracks);
        }
    }
}
