using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace DisJockey.Endpoints.Search;

public static class SearchEndpoints
{
    extension(IEndpointRouteBuilder builder)
    {
        public void MapSearchEndpoints()
        {
            var group = builder.MapGroup("/search");
            SearchTracks.MapEndpoint(group);
        }
    }
}
