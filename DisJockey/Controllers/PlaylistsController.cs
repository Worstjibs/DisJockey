using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using DisJockey.Application.Features.Playlists.Commands;
using DisJockey.Application.Features.Playlists.Queries;
using DisJockey.Extensions;
using DisJockey.Shared.Extensions;
using DisJockey.Shared.DTOs.Track;
using DisJockey.Shared.Helpers;

namespace DisJockey.Controllers;

[Authorize]
public class PlaylistsController : BaseApiController
{
    private readonly IMediator _mediator;

    public PlaylistsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> AddPlayList(AddPlaylistCommand command)
    {
        var discordId = User.GetDiscordId();
        if (!discordId.HasValue)
        {
            return Unauthorized();
        }

        command = command with { DiscordId = discordId.Value };

        var result = await _mediator.Send(command);

        return result.Match(
            Ok,
            Problem);
    }

    [HttpGet("{youtubeId}")]
    public async Task<ActionResult<PagedList<TrackListDto>>> GetPlaylistTracks([FromQuery] PaginationParams paginationParams, string youTubeId)
    {
        var query = new GetPlaylistTracksQuery(paginationParams, youTubeId);

        var result = await _mediator.Send(query);

        return Ok(result);
    }
}