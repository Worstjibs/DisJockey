using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace DisJockey.Endpoints.Playlists;

public static class PlaylistEndpoints
{
    extension(IEndpointRouteBuilder builder)
    {
        public void MapPlaylistEndpoints()
        {
            var group = builder.MapGroup("/playlists");

            AddPlaylist.MapEndpoint(group);
            GetPlaylistTracks.MapEndpoint(group);
        }
    }
}
