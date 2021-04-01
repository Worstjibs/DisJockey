using System;
using System.Linq;
using System.Threading.Tasks;
using API.Exceptions;
using API.Discord.Interfaces;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Data.Common;

namespace API.Discord.Services {
    public class DiscordTrackService : IDiscordTrackService {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVideoDetailService _videoService;
        public DiscordTrackService(IUnitOfWork unitOfWork, IVideoDetailService videoService) {
            _videoService = videoService;
            _unitOfWork = unitOfWork;
        }

        public async Task<AppUserTrack> AddTrackAsync(ulong discordId, string url) {
            var user = await _unitOfWork.UserRepository.GetUserByDiscordIdAsync(discordId);
            if (user == null) throw new UserNotFoundException($"User with discord Id {discordId} not registered");

            if (!url.Contains("youtu.be") && !url.Contains("youtube.com")) throw new InvalidUrlException("The link provided is invalid");

            var youtubeId = GetYouTubeId(url);
            if (youtubeId == null) throw new InvalidUrlException("Something is wrong with the URL provided");

            var track = await _unitOfWork.TrackRepository.GetTrackByYoutubeIdAsync(youtubeId);

            if (track == null) {
                track = new Track {
                    YoutubeId = youtubeId,
                    CreatedOn = DateTime.UtcNow
                };

                try {
                    track = await _videoService.GetVideoDetails(track);
                } catch (Exception e) {
                    Console.WriteLine(e);
                }
            }
            
            var userTrack = new AppUserTrack {
                AppUserId = user.Id,
                User = user,
                TrackId = track.Id,
                Track = track
            };

            user.Tracks.Add(userTrack);

            if (await _unitOfWork.Complete()) return userTrack;

            throw new DataContextException("Something went wrong saving the Track.");
        }

        public string GetYouTubeId(string url) {
            try {
                Uri uri = new Uri(url);

                if (uri == null) return null;

                string youtubeId = null;

                if (url.Contains("youtube.com")) {
                    var queryString = QueryHelpers.ParseQuery(uri.Query);

                    if (queryString.ContainsKey("v")) youtubeId = queryString["v"];
                } else if (url.Contains("youtu.be")) {
                    youtubeId = uri.Segments[1];
                }

                return youtubeId;
            } catch {
                throw new Exception("Something went wrong");
            }
        }
    }
}