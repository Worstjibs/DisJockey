using System.Collections.Generic;
using System.Threading.Tasks;
using DisJockey.Shared.DTOs.Member;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using DisJockey.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace DisJockey.Controllers {
    [Authorize]
    public class MembersController : BaseApiController {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MembersController(IUnitOfWork unitOfWork, IMapper mapper) {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberListDto>>> GetMembers() {
            var members = await _unitOfWork.UserRepository.GetMembersAsync();

            return Ok(members);
        }

        [HttpGet("{discordId}")]
        public async Task<ActionResult<MemberDetailDto>> GetMember(ulong discordId) {
            var user = await _unitOfWork.UserRepository.GetMemberByDiscordIdAsync(discordId);

            if (user == null) {
                return NotFound();
            }

            return user;
        }
    }
}