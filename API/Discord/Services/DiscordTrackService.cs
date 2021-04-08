using System;
using System.Linq;
using System.Threading.Tasks;
using API.Exceptions;
using API.Discord.Interfaces;
using API.Models;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Data.Common;
using Discord.WebSocket;

namespace API.Discord.Services {
    public class DiscordTrackService : IDiscordTrackService {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVideoDetailService _videoService;
        public DiscordTrackService(IUnitOfWork unitOfWork, IVideoDetailService videoService) {
            _videoService = videoService;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> AddTrackAsync(SocketUser discordUser, string url) {
            var user = await _unitOfWork.UserRepository.GetUserByDiscordIdAsync(discordUser.Id);

            if (user == null) {
                user = new AppUser {
                    DiscordId = discordUser.Id,
                    UserName = discordUser.Username,
                    AvatarUrl = discordUser.GetAvatarUrl()
                };
                _unitOfWork.UserRepository.AddUser(user);
            }
            
            if (user.UserName != discordUser.Username) {
                user.UserName = discordUser.Username;
            }

            if (user.AvatarUrl != discordUser.GetAvatarUrl()) {
                user.AvatarUrl = discordUser.GetAvatarUrl();
            }

            if(_unitOfWork.HasChanges()) {
                if(!await _unitOfWork.Complete()) 
                    throw new DataContextException("Something went wrong saving the user.");
            }

            if (!url.Contains("youtu.be") && !url.Contains("youtube.com")) throw new InvalidUrlException("The link provided is invalid");

            var youtubeId = GetYouTubeId(url);
            if (youtubeId == null) throw new InvalidUrlException("Something is wrong with the URL provided");

            var track = await _unitOfWork.TrackRepository.GetTrackByYoutubeIdAsync(youtubeId);

            if (track == null) {
                track = new Track {
                    YoutubeId = youtubeId
                };

                try {
                    track = await _videoService.GetVideoDetails(track);
                } catch (Exception e) {
                    Console.WriteLine(e);
                }

                _unitOfWork.TrackRepository.AddTrack(track);

                if (!await _unitOfWork.Complete()) throw new DataContextException("Something went wrong saving the Track.");
            }

            var userTrack = new AppUserTrack {
                AppUserId = user.Id,
                User = user,
                TrackId = track.Id,
                Track = track
            };

            user.Tracks.Add(userTrack);

            if (!await _unitOfWork.Complete()) throw new DataContextException("Something went wrong saving the AppUserTrack.");
            
            return true;            
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