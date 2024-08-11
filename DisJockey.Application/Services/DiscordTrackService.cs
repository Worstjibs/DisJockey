using DisJockey.Shared.Exceptions;
using DisJockey.Core;
using DisJockey.Services.Interfaces;
using static DisJockey.Services.Interfaces.IDiscordTrackService;
using DisJockey.Application.Interfaces;

namespace DisJockey.Application.Services;

public class DiscordTrackService : IDiscordTrackService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IVideoDetailService _videoService;

    public DiscordTrackService(
        IUnitOfWork unitOfWork,
        IVideoDetailService videoService
    )
    {
        _videoService = videoService;
        _unitOfWork = unitOfWork;
    }

    public async Task AddTrackAsync(AddTrackArgs args, string youtubeId)
    {
        var user = await _unitOfWork.UserRepository.GetUserByDiscordIdAsync(args.DiscordId);

        if (user == null)
        {
            user = CreateAppUser(args);
            _unitOfWork.UserRepository.AddUser(user);
        }
        else
        {
            user.AvatarUrl = args.AvatarUrl;
            user.UserName = args.Username;
        }

        if (_unitOfWork.HasChanges())
        {
            if (!await _unitOfWork.Complete())
                throw new DataContextException("Something went wrong saving the user.");
        }

        if (await _unitOfWork.TrackRepository.IsTrackBlacklisted(youtubeId))
        {
            throw new Exception("This track is blacklisted.");
        }

        var track = await _unitOfWork.TrackRepository.GetTrackByYoutubeIdAsync(youtubeId);

        if (track == null)
        {
            track = new Track
            {
                YoutubeId = youtubeId,
                CreatedOn = DateTime.UtcNow,
                TrackPlays = new List<TrackPlay>()
            };

            try
            {
                track = await _videoService.GetVideoDetailsAsync(track);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            _unitOfWork.TrackRepository.AddTrack(track);

            if (!await _unitOfWork.Complete()) throw new DataContextException("Something went wrong saving the Track.");
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
                TrackPlayHistory = new List<TrackPlayHistory>()
            };
            track.TrackPlays.Add(trackPlay);
        }

        trackPlay.LastPlayed = DateTime.UtcNow;

        trackPlay.TrackPlayHistory.Add(new TrackPlayHistory
        {
            CreatedOn = trackPlay.LastPlayed,
            TrackPlay = trackPlay
        });

        if (!await _unitOfWork.Complete()) throw new DataContextException("Something went wrong saving the AppUserTrack.");
    }

    private static AppUser CreateAppUser(AddTrackArgs args)
    {
        var user = new AppUser
        {
            DiscordId = args.DiscordId,
            UserName = args.Username,
            AvatarUrl = args.AvatarUrl,
            Tracks = new List<TrackPlay>()
        };
        return user;
    }
}