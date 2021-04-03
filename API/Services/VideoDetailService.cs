using System;
using System.Linq;
using System.Threading.Tasks;
using API.Models;
using API.Helpers;
using API.Interfaces;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.Extensions.Options;

namespace API.Services {
    public class VideoDetailService : IVideoDetailService {
        private readonly YouTubeService _youTubeService;
        public VideoDetailService(IOptions<YoutubeSettings> config) {
            _youTubeService = new YouTubeService(new BaseClientService.Initializer() {
                ApiKey = config.Value.ApiKey
            });
        }

        public async Task<Track> GetVideoDetails(Track track) {
            var searchRequest = _youTubeService.Videos.List("snippet");
            searchRequest.Id = track.YoutubeId;

            var searchResponse = await searchRequest.ExecuteAsync();

            var youtubeVideo = searchResponse.Items.FirstOrDefault();

            if (youtubeVideo != null) {
                track.Title = youtubeVideo.Snippet.Title;
                track.Description = youtubeVideo.Snippet.Description;
                track.ChannelTitle = youtubeVideo.Snippet.ChannelTitle;
                track.SmallThumbnail = youtubeVideo.Snippet.Thumbnails.Medium.Url;
                track.MediumThumbnail = youtubeVideo.Snippet.Thumbnails.High.Url;
                track.LargeThumbnail = youtubeVideo.Snippet.Thumbnails.Standard.Url;

                return track;
            }

            throw new Exception("Invalid Youtube Id");
        }
    }
}