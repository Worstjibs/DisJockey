using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace DisJockey.Endpoints.Tracks;

public static class TrackEndpoints
{
    extension(IEndpointRouteBuilder builder)
    {
        public void MapTrackEndpoints()
        {
            var group = builder.MapGroup("/tracks");

            BlacklistTrack.MapEndpoint(group);
            LikeTrack.MapEndpoint(group);
            PlayTrack.MapEndpoint(group);
            GetAllTracks.MapEndpoint(group);
            GetTracksForMember.MapEndpoint(group);
        }
    }
}
