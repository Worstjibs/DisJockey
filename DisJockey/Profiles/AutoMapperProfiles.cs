using System;
using System.Linq;
using DisJockey.Shared.DTOs;
using DisJockey.Shared.DTOs.Member;
using DisJockey.Shared.DTOs.Playlist;
using DisJockey.Shared.DTOs.Shared;
using DisJockey.Shared.DTOs.Track;
using DisJockey.Core;
using AutoMapper;
using DisJockey.Shared.DTOs.PullUps;

namespace DisJockey.Profiles
{
    public class AutoMapperProfiles : Profile
    {

        public AutoMapperProfiles()
        {
            ulong? DiscordId = null;

            CreateTrackListMappings(DiscordId);

            CreateMemberListMappings();

            CreatePlaylistMappings();

            CreatePullUpMappings();
        }

        private void CreateTrackListMappings(ulong? DiscordId)
        {
            CreateMap<Track, BaseTrackDto>()
                .ForMember(dest => dest.Likes, opt => opt.MapFrom(src => src.Likes.Where(x => x.Liked == true).Count()))
                .ForMember(dest => dest.Dislikes, opt => opt.MapFrom(src => src.Likes.Where(x => x.Liked == false).Count()))
                .ForMember(dest => dest.LikedByUser, opt => opt.MapFrom(src => src.Likes.FirstOrDefault(x => x.User.DiscordId == DiscordId).Liked));

            CreateMap<Track, TrackListDto>()
                .IncludeBase<Track, BaseTrackDto>()
                .ForMember(dest => dest.Users, opt => opt.MapFrom(src => src.TrackPlays.OrderByDescending(tp => tp.LastPlayed)))
                .ForMember(dest => dest.UserLikes, opt => opt.MapFrom(src => src.Likes))
                .ForMember(dest => dest.LastPlayed, opt => opt.MapFrom(src => src.TrackPlays.Count > 0 ? src.TrackPlays.Max(x => x.LastPlayed) : DateTime.MinValue));

            // Mappings for TrackPlay
            CreateMap<TrackPlay, TrackPlayDto>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.DiscordId, opt => opt.MapFrom(src => src.User.DiscordId))
                .ForMember(dest => dest.TimesPlayed, opt => opt.MapFrom(src => src.TrackPlayHistory.Count))
                .ForMember(dest => dest.LastPlayed, opt => opt.MapFrom(src => src.LastPlayed))
                .ForMember(dest => dest.FirstPlayed, opt => opt.MapFrom(src => src.CreatedOn))
                .ForMember(dest => dest.History, opt => opt.MapFrom(src => src.TrackPlayHistory));

            // Mappings for TrackPlayHistory
            CreateMap<TrackPlayHistory, TrackPlayHistoryDto>();

            // Mappings for TrackUserLike
            CreateMap<TrackLike, TrackUserLikeDto>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.DiscordId, opt => opt.MapFrom(src => src.User.DiscordId))
                .ForMember(dest => dest.Liked, opt => opt.MapFrom(src => src.Liked));
        }

        private void CreateMemberListMappings()
        {
            CreateMap<AppUser, BaseMemberDto>()
                .ForMember(dest => dest.DateJoined, opt => opt.MapFrom(src => src.CreatedOn));

            CreateMap<AppUser, MemberListDto>()
                .IncludeBase<AppUser, BaseMemberDto>()
                .ForMember(dest => dest.TracksPlayed, opt => opt.MapFrom(src => src.Tracks.Count));

            CreateMap<AppUser, MemberDetailDto>()
                .IncludeBase<AppUser, MemberListDto>()
                .ForMember(dest => dest.Playlists, opt => opt.MapFrom(src => src.Playlists));
        }

        private void CreatePlaylistMappings()
        {
            CreateMap<Playlist, BasePlaylistDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.YoutubeId, opt => opt.MapFrom(src => src.YoutubeId));

            CreateMap<Playlist, PlaylistDetailDto>()
                .IncludeBase<Playlist, BasePlaylistDto>()
                .ForMember(dest => dest.Tracks, opt => opt.MapFrom(src => src.Tracks));

            CreateMap<PlaylistTrack, BaseTrackDto>()
                .ForMember(dest => dest.ChannelTitle, opt => opt.MapFrom(src => src.Track.ChannelTitle))
                .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => src.Track.CreatedOn))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Track.Description))
                .ForMember(dest => dest.LargeThumbnail, opt => opt.MapFrom(src => src.Track.LargeThumbnail))
                .ForMember(dest => dest.MediumThumbnail, opt => opt.MapFrom(src => src.Track.MediumThumbnail))
                .ForMember(dest => dest.SmallThumbnail, opt => opt.MapFrom(src => src.Track.SmallThumbnail))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Track.Title))
                .ForMember(dest => dest.YoutubeId, opt => opt.MapFrom(src => src.Track.YoutubeId));
        }

        private void CreatePullUpMappings()
        {
            CreateMap<Track, PullUpDto>()
                .IncludeBase<Track, BaseTrackDto>()
                .ForMember(dest => dest.LastPulled, opt => opt.MapFrom(src => src.PullUps.Max(x => x.CreatedOn)))
                .ForMember(dest => dest.PullUps, opt => opt.MapFrom(src => src.PullUps));

            CreateMap<PullUp, PullUpTrackDto>()
                .ForMember(dest => dest.DatePulled, opt => opt.MapFrom(src => src.CreatedOn))
                .ForMember(dest => dest.TimePulled, opt => opt.MapFrom(src => src.TimePulled));
        }
    }
}