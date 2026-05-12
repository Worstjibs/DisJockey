using DisJockey.Shared.Exceptions;
using DisJockey.Core;
using DisJockey.Services.Interfaces;
using static DisJockey.Application.Interfaces.IDiscordTrackService;
using DisJockey.Application.Interfaces;

namespace DisJockey.Application.Services;

public class DiscordTrackService : IDiscordTrackService
{
    private readonly IUserRepository _userRepository;
    private readonly ITrackRepository _trackRepository;
    private readonly IVideoDetailService _videoService;

    public DiscordTrackService(
        IUserRepository userRepository,
        ITrackRepository trackRepository,
        IVideoDetailService videoService)
    {
        _videoService = videoService;
        _userRepository = userRepository;
        _trackRepository = trackRepository;
    }

    public async Task AddTrackAsync(AddTrackArgs args, string youtubeId)
    {
        var user = await _userRepository.GetUserByDiscordIdAsync(args.DiscordId);
        if (user == null)
        {
            user = await CreateAppUser(args);
        }
        else
        {
            user.AvatarUrl = args.AvatarUrl;
            user.UserName = args.Username;
        }

        if (await _trackRepository.IsTrackBlacklisted(youtubeId))
        {
            throw new Exception("This track is blacklisted.");
        }

        var track = await _trackRepository.GetTrackByYoutubeIdAsync(youtubeId);

        if (track == null)
        {
            track = new Track
            {
                YoutubeId = youtubeId,
                CreatedOn = DateTime.UtcNow,
                TrackPlays = []
            };

            try
            {
                track = await _videoService.GetVideoDetailsAsync(track);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            _trackRepository.AddTrack(track);

            if (!await _trackRepository.SaveChangesAsync())
            {
                throw new DataContextException("Something went wrong saving the Track.");
            }
        }

        var trackPlay = track.TrackPlays.FirstOrDefault(x => x.AppUserId == user.Id);

        if (trackPlay == null)
        {
            trackPlay = new TrackPlay
            {
                AppUserId = user.Id,
                User = user,
                TrackId = track.Id,
                Track = track,
                TrackPlayHistory = []
            };
            track.TrackPlays.Add(trackPlay);
        }

        trackPlay.LastPlayed = DateTime.UtcNow;

        trackPlay.TrackPlayHistory.Add(new TrackPlayHistory
        {
            CreatedOn = trackPlay.LastPlayed,
            TrackPlay = trackPlay
        });

        if (!await _trackRepository.SaveChangesAsync())
        {
            throw new DataContextException("Something went wrong saving the AppUserTrack.");
        }
    }

    private async Task<AppUser> CreateAppUser(AddTrackArgs args)
    { 
        var user = new AppUser
        {
            DiscordId = args.DiscordId,
            UserName = args.Username,
            AvatarUrl = args.AvatarUrl,
            Tracks = []
        };

        _userRepository.AddUser(user);

        var result = await _userRepository.SaveChangesAsync();
        if (!result)
        {
            throw new DataContextException("Something went wrong saving the user.");
        }

        return user;
    }
}