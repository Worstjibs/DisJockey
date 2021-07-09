using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.Member;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers {
    public class MembersController : BaseApiController {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MembersController(IUnitOfWork unitOfWork, IMapper mapper) {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetMembers() {
            var members = await _unitOfWork.UserRepository.GetMembersAsync();

            // Doing grouping "client-side" here
            // TODO figure out how to do it during projection
            foreach(var member in members) {
                member.Tracks = member.Tracks.GroupBy(t => t.YoutubeId).Select(t => 
                    new MemberTrackDto {
                        YoutubeId = t.Key,
                        FirstPlayed = t.Min(x => x.FirstPlayed),
                        LastPlayed = t.Max(x => x.LastPlayed),
                        TimesPlayed = t.Count()
                    }).ToList();
            }

            return Ok(members);
        }

        [HttpGet("{discordId}")]
        public async Task<ActionResult<MemberDto>> GetMemberByUsername(ulong discordId) {
            return await _unitOfWork.UserRepository.GetMemberByDiscordIdAsync(discordId);
        }
    }
}