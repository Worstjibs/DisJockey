using DisJockey.Core;
using DisJockey.Shared.DTOs.Playlist;
using DisJockey.Shared.DTOs.Shared;

namespace DisJockey.Application.Mappers;

public static class PlaylistMapper
{
    extension(Playlist playlist)
    {
        public PlaylistDetailDto ToDetailDto() => new()
        {
            YoutubeId = playlist.YoutubeId,
            Name = playlist.Name,
            Tracks = playlist.Tracks
                .Select(pt => new BaseTrackDto
                {
                    YoutubeId = pt.Track.YoutubeId,
                    Title = pt.Track.Title,
                    Description = pt.Track.Description,
                    ChannelTitle = pt.Track.ChannelTitle,
                    CreatedOn = pt.Track.CreatedOn,
                    SmallThumbnail = pt.Track.SmallThumbnail,
                    MediumThumbnail = pt.Track.MediumThumbnail,
                    LargeThumbnail = pt.Track.LargeThumbnail
                })
                .ToList()
        };
    }
}
