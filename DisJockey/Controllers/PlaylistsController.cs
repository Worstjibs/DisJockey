using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using DisJockey.Application.Features.Playlists.Commands;
using DisJockey.Extensions;
using DisJockey.Shared.Extensions;

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
}