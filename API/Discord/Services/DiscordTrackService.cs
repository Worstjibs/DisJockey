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
using API.Extensions;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace API.Discord.Services {
    public class DiscordTrackService : IDiscordTrackService {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVideoDetailService _videoService;
        private readonly MusicService _musicService;
        public DiscordTrackService(IUnitOfWork unitOfWork, IVideoDetailService videoService, MusicService musicService) {
            _musicService = musicService;
            _videoService = videoService;
            _unitOfWork = unitOfWork;
        }

        public async Task AddTrackAsync(SocketUser discordUser, string url) {
            var user = await _unitOfWork.UserRepository.GetUserByDiscordIdAsync(discordUser.Id);

            if (user == null) {
                user = CreateAppUser(discordUser);
                _unitOfWork.UserRepository.AddUser(user);
            } else {
                user.UpdateAppUser(discordUser);
            }

            if (_unitOfWork.HasChanges()) {
                if (!await _unitOfWork.Complete())
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
        }

        public async Task PullUpTrackAsync(SocketUser discordUser, string url, double currentPosition) {
            var user = await _unitOfWork.UserRepository.GetUserByDiscordIdAsync(discordUser.Id);

            if (user == null) {
                user = CreateAppUser(discordUser);
                _unitOfWork.UserRepository.AddUser(user);
            } else {
                user.UpdateAppUser(discordUser);
            }

            if (_unitOfWork.HasChanges()) {
                if (!await _unitOfWork.Complete())
                    throw new DataContextException("Something went wrong saving the user.");
            }

            if (!url.Contains("youtu.be") && !url.Contains("youtube.com")) throw new InvalidUrlException("The link provided is invalid");

            var youtubeId = GetYouTubeId(url);
            if (youtubeId == null) throw new InvalidUrlException("Something is wrong with the URL provided");

            var track = await _unitOfWork.TrackRepository.GetTrackByYoutubeIdAsync(youtubeId);

            if (track == null) {
                throw new Exception("Cannot find the track with Youtube Id " + youtubeId);
            }

            var pullUp = new PullUp {
                TrackId = track.Id,
                Track = track,
                UserId = user.Id,
                User = user,
                TimePulled = currentPosition
            };

            track.PullUps.Add(pullUp);

            if (!await _unitOfWork.Complete()) {
                throw new DataContextException("Something went wrong saving the PullUp");
            }
        }               

        private AppUser CreateAppUser(SocketUser discordUser) {
            var user = new AppUser {
                DiscordId = discordUser.Id,
                UserName = discordUser.Username,
                AvatarUrl = discordUser.GetAvatarUrl(),
                Tracks = new List<AppUserTrack>()
            };
            return user;
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