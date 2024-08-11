using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using DisJockey.Application.Features.Search.Queries;
using DisJockey.Extensions;
using DisJockey.Shared.DTOs.Track;
using DisJockey.Shared.Helpers;

namespace DisJockey.Controllers;

[Authorize]
public class SearchController : BaseApiController
{
    private readonly IMediator _mediator;

    public SearchController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TrackListDto>>> SearchTracks([FromQuery] PaginationParams paginationParams)
    {
        var query = new SearchQuery(paginationParams);

        var results = await _mediator.Send(query);

        var pagination = results.Pagination;

        Response.AddYouTubePaginationHeader(pagination.CurrentPageToken, pagination.NextPageToken, pagination.PreviousPageToken);

        return Ok(results.Results);
    }
}
