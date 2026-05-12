using DisJockey.Application.Contracts;
using DisJockey.Application.Features.Search.Queries;
using DisJockey.Extensions;
using DisJockey.Shared.DTOs.Track;
using DisJockey.Shared.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DisJockey.Endpoints.Search;

public static class SearchTracks
{
    extension(IEndpointRouteBuilder builder)
    {
        public IEndpointRouteBuilder MapEndpoint()
        {
            builder.MapGet(
                "/",
                async (
                    IMediator mediator,
                    HttpContext httpContext,
                    CancellationToken cancellationToken,
                    int? pageSize = null,
                    string? query = null,
                    string? sortBy = null) =>
                {
                    var pagination = PaginationParams.CreateParameters(pageNumber: null, pageSize, sortBy, query);

                    var (results, youtubePagination) = await mediator.SendAsync<SearchQuery, (IEnumerable<TrackListDto>, YouTubePagination?)>(
                        new SearchQuery(pagination), cancellationToken);

                    if (youtubePagination is not null)
                    {
                        httpContext.Response.AddYouTubePaginationHeader(
                            youtubePagination.CurrentPageToken,
                            youtubePagination.NextPageToken,
                            youtubePagination.PreviousPageToken);
                    }

                    return Results.Ok(results);
                })
                .RequireAuthorization();

            return builder;
        }
    }
}
