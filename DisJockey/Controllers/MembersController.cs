using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DisJockey.Shared.DTOs.Member;
using DisJockey.Application.Members.Queries.AllMembers;
using MediatR;
using DisJockey.Application.Members.Queries.GetMember;

namespace DisJockey.Controllers;

[Authorize]
public class MembersController : BaseApiController
{
    private readonly IMediator _mediator;

    public MembersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberListDto>>> GetMembers()
    {
        var members = await _mediator.Send(new AllMembersQuery());

        return Ok(members);
    }

    [HttpGet("{discordId}")]
    public async Task<ActionResult<MemberDetailDto>> GetMember(ulong discordId)
    {
        var member = await _mediator.Send(new GetMemberQuery(discordId));

        if (member is null)
            return NotFound();

        return Ok(member);
    }
}