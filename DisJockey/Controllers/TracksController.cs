using System.Threading.Tasks;
using DisJockey.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DisJockey.Shared.DTOs.Track;
using DisJockey.Shared.Helpers;
using DisJockey.Shared.Extensions;
using MediatR;
using DisJockey.Application.Features.Tracks.Commands.BlacklistTrack;
using DisJockey.Application.Features.Tracks.Commands.LikeTrack;
using DisJockey.Application.Features.Tracks.Commands.PlayTrack;
using DisJockey.Application.Features.Tracks.Queries.AllTracks;
using DisJockey.Application.Features.Tracks.Queries.TracksForMember;

namespace DisJockey.Controllers;

[Authorize]
public class TracksController : BaseApiController
{
    private readonly IMediator _mediator;

    public TracksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<PagedList<TrackListDto>>> GetTracks([FromQuery] PaginationParams paginationParams)
    {
        var tracks = await _mediator.Send(new AllTracksQuery(paginationParams));

        Response.AddPaginationHeader(tracks.CurrentPage, tracks.ItemsPerPage, tracks.TotalPages, tracks.TotalCount);

        return Ok(tracks);
    }

    [HttpGet("{discordId}")]
    public async Task<ActionResult<PagedList<TrackListDto>>> GetTrackPlaysForMember([FromQuery] PaginationParams paginationParams, ulong discordId)
    {
        var tracks = await _mediator.Send(new TracksForMemberQuery(paginationParams, discordId));

        Response.AddPaginationHeader(tracks.CurrentPage, tracks.ItemsPerPage, tracks.TotalPages, tracks.TotalCount);

        return Ok(tracks);
    }

    [HttpPost("like")]
    public async Task<IActionResult> LikeTrack([FromBody] LikeTrackCommand command)
    {
        var discordId = User.GetDiscordId();
        if (!discordId.HasValue)
        {
            return Unauthorized();
        }

        command = command with { DiscordId = discordId.Value };

        var result = await _mediator.Send(command);

        return result.Match(
            Success => Ok(),
            Problem);
    }

    [HttpPost("play")]
    public async Task<IActionResult> PlayTrack([FromBody] PlayTrackCommand command)
    {
        var discordId = User.GetDiscordId();
        if (!discordId.HasValue)
        {
            return Unauthorized();
        }

        command = command with { DiscordId = discordId.Value };

        var result = await _mediator.Send(command);

        return result.Match(
            Success => Ok(),
            Problem);
    }

    [HttpPut("{id}/blacklist")]
    public async Task<IActionResult> BlacklistTrack(int id)
    {
        var command = new BlacklistTrackCommand(id);

        var result = await _mediator.Send(command);

        return result.Match(
            Success => NoContent(),
            Problem);
    }
}