using DisJockey.Application.Features.Tracks.Commands.BlacklistTrack;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Threading;

namespace DisJockey.Endpoints.Tracks;

public static class BlacklistTrack
{
    extension(IEndpointRouteBuilder builder)
    {
        public IEndpointRouteBuilder MapEndpoint()
        {
            builder.MapPut(
                "/api/tracks/{id}/blacklist",
                async (
                    IMediator mediator, 
                    int id,
                    CancellationToken cancellationToken) =>
                {
                    var command = new BlacklistTrackCommand(id);

                    var result = await mediator.Send(command, cancellationToken);

                    return result.Match(
                        success => Results.NoContent(),
                        errors => errors.Problem());
                })
                .RequireAuthorization();

            return builder;
        }
    }
}
