using Microsoft.AspNetCore.Routing;

namespace DisJockey.Endpoints.Tracks;

public static class TrackEndpoints
{
    extension(IEndpointRouteBuilder builder)
    {
        public void MapTrackEndpoints()
        {
            BlacklistTrack.MapEndpoint(builder);
            LikeTrack.MapEndpoint(builder);
            PlayTrack.MapEndpoint(builder);
            GetAllTracks.MapEndpoint(builder);
            GetTracksForMember.MapEndpoint(builder);
        }
    }
}
