using DisJockey.Application.Contracts;
using DisJockey.Application.Features.Tracks.Commands.PlayTrack;
using DisJockey.Shared.Extensions;
using ErrorOr;
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
                "/play",
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

                    var result = await mediator.SendAsync<PlayTrackCommand, ErrorOr<Success>>(command, cancellationToken);

                    return result.Match(
                        success => Results.Ok(),
                        errors => errors.Problem());
                })
                .RequireAuthorization();

            return builder;
        }
    }
}
