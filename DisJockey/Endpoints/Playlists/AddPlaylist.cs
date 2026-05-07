using DisJockey.Application.Features.Playlists.Commands;
using DisJockey.Shared.Extensions;
using DisJockey.Shared.DTOs.Playlist;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;
using System.Threading;
using DisJockey.Application.Contracts;
using ErrorOr;

namespace DisJockey.Endpoints.Playlists;

public static class AddPlaylist
{
    extension(IEndpointRouteBuilder builder)
    {
        public IEndpointRouteBuilder MapEndpoint()
        {
            builder.MapPost(
                "/",
                async (
                    IMediator mediator,
                    ClaimsPrincipal user,
                    AddPlaylistCommand command,
                    CancellationToken cancellationToken) =>
                {
                    var discordId = user.GetDiscordId();
                    if (!discordId.HasValue)
                    {
                        return Results.Unauthorized();
                    }

                    command = command with { DiscordId = discordId.Value };

                    var result = await mediator.SendAsync<AddPlaylistCommand, ErrorOr<PlaylistDetailDto>>(command, cancellationToken);

                    return result.Match(
                        playlist => Results.Ok(playlist),
                        errors => errors.Problem());
                })
                .RequireAuthorization();

            return builder;
        }
    }
}
