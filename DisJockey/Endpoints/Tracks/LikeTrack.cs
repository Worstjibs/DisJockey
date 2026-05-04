using DisJockey.Application.Features.Tracks.Commands.LikeTrack;
using DisJockey.Shared.Extensions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace DisJockey.Endpoints.Tracks;

public static class LikeTrack
{
    extension(IEndpointRouteBuilder builder)
    {
        public IEndpointRouteBuilder MapEndpoint()
        {
            builder.MapPost(
                "api/tracks/like",
                async (IMediator mediator,
                ClaimsPrincipal claimsPrincipal,
                LikeTrackCommand command) =>
                {
                    var discordId = claimsPrincipal.GetDiscordId();
                    if (!discordId.HasValue)
                    {
                        return Results.Unauthorized();
                    }

                    command = command with { DiscordId = discordId.Value };

                    var result = await mediator.Send(command);

                    return result.Match(
                        success => Results.Ok(),
                        errors => errors.Problem());
                })
                .RequireAuthorization();

            return builder;
        }
    }
}
