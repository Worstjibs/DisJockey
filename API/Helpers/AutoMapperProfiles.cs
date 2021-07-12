using System;
using System.Linq;
using System.Reflection;
using API.DTOs;
using API.DTOs.Member;
using API.DTOs.Playlist;
using API.DTOs.Shared;
using API.DTOs.Track;
using API.Entities;
using API.Interfaces;
using AutoMapper;

namespace API.Helpers {
    public class AutoMapperProfiles : Profile {
        public AutoMapperProfiles() {
            CreateTrackListMappings();

            CreateMemberListMappings();

            CreatePlaylistMappings();
        }

        private void CreateTrackListMappings() {
            CreateMap<Track, BaseTrackDto>();

            CreateMap<Track, TrackListDto>()
                .ForMember(dest => dest.Users, opt => opt.MapFrom(src => src.TrackPlays.OrderByDescending(tp => tp.LastPlayed)))
                .ForMember(dest => dest.UserLikes, opt => opt.MapFrom(src => src.Likes))
                .ForMember(dest => dest.Likes, opt => opt.MapFrom(src => src.Likes.Where(x => x.Liked == true).Count()))
                .ForMember(dest => dest.Dislikes, opt => opt.MapFrom(src => src.Likes.Where(x => x.Liked == false).Count()))
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

        private void CreateMemberListMappings() {
            CreateMap<AppUser, MemberListDto>()
                .ForMember(dest => dest.DateJoined, opt => opt.MapFrom(src => src.CreatedOn))
                .ForMember(dest => dest.DiscordId, opt => opt.MapFrom(src => src.DiscordId))
                .ForMember(dest => dest.TracksPlayed, opt => opt.MapFrom(src => src.Tracks.Count));

            CreateMap<AppUser, MemberDetailDto>()
                .IncludeBase<AppUser, MemberListDto>()
                .ForMember(dest => dest.Playlists, opt => opt.MapFrom(src => src.Playlists));
        }

        private void CreatePlaylistMappings() {
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
    }
}