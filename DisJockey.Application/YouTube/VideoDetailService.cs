using DisJockey.Core;
using DisJockey.Shared.Helpers;
using DisJockey.Services.Interfaces;
using Microsoft.Extensions.Options;
using GoogleData = Google.Apis.YouTube.v3.Data;
using System.Web;
using Google.Apis.YouTube.v3;
using Google.Apis.Services;

namespace DisJockey.Application.YouTube;

public class VideoDetailService : IVideoDetailService
{
    private const int MAX_ITEMS = 10000;

    private readonly YouTubeService _youTubeService;
    public VideoDetailService(IOptions<YoutubeSettings> config)
    {
        _youTubeService = new YouTubeService(new BaseClientService.Initializer()
        {
            ApiKey = config.Value.ApiKey
        });
    }

    public async Task<Playlist> GetPlaylistDetailsAsync(string playlistId)
    {
        var playlistRequest = _youTubeService.Playlists.List("snippet,contentDetails");
        playlistRequest.Id = playlistId;

        var playlistResponse = await playlistRequest.ExecuteAsync();

        if (playlistResponse?.Items.Count == 0)
        {
            return null;
        }

        var playlistResult = playlistResponse.Items.First();

        var playlist = new Playlist
        {
            Name = playlistResult.Snippet.Title,
            YoutubeId = playlistId,
            Tracks = new List<PlaylistTrack>()
        };

        var pageToken = "";
        var playlistItemCount = playlistResult.ContentDetails.ItemCount;

        do
        {
            var playlistItemsRequest = _youTubeService.PlaylistItems.List("snippet,contentDetails");
            playlistItemsRequest.PlaylistId = playlistId;
            playlistItemsRequest.MaxResults = playlistItemCount < MAX_ITEMS ? playlistItemCount : MAX_ITEMS;
            playlistItemsRequest.PageToken = pageToken;

            var playlistItemsResponse = await playlistItemsRequest.ExecuteAsync();

            var playlistTracks = playlistItemsResponse?.Items;

            ProcessTracks(playlist, playlistTracks);
            pageToken = playlist.Tracks.Count >= MAX_ITEMS || playlist.Tracks.Count == playlistItemCount ? null : playlistItemsResponse.NextPageToken;
        } while (pageToken != null);

        return playlist;
    }

    public async Task<Track> GetVideoDetailsAsync(Track track)
    {
        var searchRequest = _youTubeService.Videos.List("snippet");
        searchRequest.Id = track.YoutubeId;

        var searchResponse = await searchRequest.ExecuteAsync();

        var youtubeVideo = searchResponse.Items.FirstOrDefault();

        if (youtubeVideo != null)
        {
            track.Title = youtubeVideo.Snippet.Title;
            track.Description = youtubeVideo.Snippet.Description;
            track.ChannelTitle = youtubeVideo.Snippet.ChannelTitle;
            track.SmallThumbnail = youtubeVideo.Snippet.Thumbnails.Medium?.Url;
            track.MediumThumbnail = youtubeVideo.Snippet.Thumbnails.High?.Url;
            track.LargeThumbnail = youtubeVideo.Snippet.Thumbnails.Standard?.Url;

            return track;
        }

        throw new Exception("Invalid Youtube Id");
    }

    public async Task<YouTubePagedList<Track>> QueryTracksAsync(PaginationParams paginationParams)
    {
        var searchRequest = _youTubeService.Search.List("snippet");
        searchRequest.Q = paginationParams.Query;
        searchRequest.Type = "video";
        searchRequest.MaxResults = paginationParams.PageSize;
        searchRequest.PageToken = paginationParams.PageToken;

        var searchResponse = await searchRequest.ExecuteAsync();

        if (!searchResponse.Items.Any())
        {
            return YouTubePagedList<Track>.Empty();
        }

        var tracks = searchResponse.Items.Select(MapSearchResultToTrack);

        var pagedList = new YouTubePagedList<Track>(tracks, paginationParams.PageToken, searchResponse.NextPageToken, searchResponse.PrevPageToken);

        return pagedList;
    }


    private static void ProcessTracks(Playlist playlist, IList<GoogleData.PlaylistItem> playlistItems)
    {
        var playlistTracks = playlistItems.Select(x => new PlaylistTrack
        {
            CreatedOn = DateTime.UtcNow,
            Playlist = playlist,
            Track = new Track
            {
                YoutubeId = x.ContentDetails.VideoId,
                Title = x.Snippet.Title,
                Description = x.Snippet.Description,
                ChannelTitle = x.Snippet.VideoOwnerChannelTitle,
                SmallThumbnail = x.Snippet.Thumbnails.Medium?.Url,
                MediumThumbnail = x.Snippet.Thumbnails.High?.Url,
                LargeThumbnail = x.Snippet.Thumbnails.Standard?.Url
            }
        });

        ((List<PlaylistTrack>)playlist.Tracks).AddRange(playlistTracks);
    }

    private Track MapSearchResultToTrack(GoogleData.SearchResult result)
    {
        var snippet = result.Snippet;

        return new Track
        {
            YoutubeId = result.Id.VideoId,
            Title = HttpUtility.HtmlDecode(snippet.Title),
            Description = snippet.Description,
            ChannelTitle = snippet.ChannelTitle,
            SmallThumbnail = snippet.Thumbnails.Medium?.Url,
            MediumThumbnail = snippet.Thumbnails.Medium?.Url,
            LargeThumbnail = snippet.Thumbnails.High?.Url,
            TrackPlays = new List<TrackPlay>(),
            Likes = new List<TrackLike>()
        };
    }
}