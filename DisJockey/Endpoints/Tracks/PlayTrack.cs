using DisJockey.Application.Features.Tracks.Commands.PlayTrack;
using DisJockey.Shared.Extensions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;
using System.Threading;

namespace DisJockey.Endpoints.Tracks;

public static class PlayTrack
{
    extension(IEndpointRouteBuilder builder)
    {
        public IEndpointRouteBuilder MapEndpoint()
        {
            builder.MapPost(
                "/api/tracks/play",
                async (
                    IMediator mediator,
                    PlayTrackCommand command,
                    ClaimsPrincipal user,
                    CancellationToken cancellationToken) =>
                {
                    var discordId = user.GetDiscordId();
                    if (!discordId.HasValue)
                    {
                        return Results.Unauthorized();
                    }

                    command = command with { DiscordId = discordId.Value };

                    var result = await mediator.Send(command, cancellationToken);

                    return result.Match(
                        success => Results.Ok(),
                        errors => errors.Problem());
                })
                .RequireAuthorization();

            return builder;
        }
    }
}
