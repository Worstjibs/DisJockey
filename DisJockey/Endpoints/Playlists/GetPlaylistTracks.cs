using DisJockey.Application.Contracts;
using DisJockey.Application.Features.Playlists.Queries;
using DisJockey.Shared.DTOs.Track;
using DisJockey.Shared.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Threading;

namespace DisJockey.Endpoints.Playlists;

public static class GetPlaylistTracks
{
    extension(IEndpointRouteBuilder builder)
    {
        public IEndpointRouteBuilder MapEndpoint()
        {
            builder.MapGet(
                "/{youtubeId}",
                async (
                    IMediator mediator,
                    [AsParameters] GetPlaylistTracksRequest request,
                    CancellationToken cancellationToken) =>
                {
                    var pagination = PaginationParams.CreateParameters(
                        request.PageNumber,
                        request.PageSize,
                        request.SortBy,
                        request.Query);

                    var tracks = await mediator.SendAsync<GetPlaylistTracksQuery, PagedList<TrackListDto>>(
                        new GetPlaylistTracksQuery(pagination, request.YouTubeId),
                        cancellationToken);

                    return Results.Ok(tracks);
                })
                .RequireAuthorization();

            return builder;
        }
    }
}

public class GetPlaylistTracksRequest
{
    [FromQuery]
    public int? PageNumber { get; set; }

    [FromQuery]
    public int? PageSize { get; set; }

    [FromQuery]
    public string? Query { get; set; }

    [FromQuery]
    public string? SortBy { get; set; }

    [FromRoute]
    public required string YouTubeId { get; set; }
}