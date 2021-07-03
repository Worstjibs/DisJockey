using System;
using System.Linq;
using System.Reflection;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;

namespace API.Helpers {
    public class AutoMapperProfiles : Profile {
        public AutoMapperProfiles() {
            CreateMap<AppUser, MemberDto>()
                .ForMember(dest => dest.DateJoined, opt => opt.MapFrom(src => src.CreatedOn));

            CreateMap<AppUser, TrackPlayDto>();

            CreateMap<Track, MemberTrackDto>();
                // .ForMember(dest => dest.Likes, opt => opt.MapFrom(src => src.Likes.Where(x => x.Liked == true).Count()))
                // .ForMember(dest => dest.Dislikes, opt => opt.MapFrom(src => src.Likes.Where(x => x.Liked == false).Count()));

            CreateMap<Track, TrackDto>()
                .ForMember(dest => dest.Users, opt => opt.MapFrom(src => src.TrackPlays.OrderByDescending(tp => tp.LastPlayed)))
                    // .GroupBy(x => new {x.User.DiscordId, x.User.UserName}).Select(t => new TrackUserDto {
                    //     CreatedOn = t.Min(x => x.CreatedOn),
                    //     LastPlayed =  t.Max(x => x.CreatedOn),
                    //     TimesPlayed = t.Count(),
                    //     DiscordId = t.Key.DiscordId,
                    //     Username = t.Key.UserName    
                    // })))
                .ForMember(dest => dest.UserLikes, opt => opt.MapFrom(src => src.Likes))
                .ForMember(dest => dest.Likes, opt => opt.MapFrom(src => src.Likes.Where(x => x.Liked == true).Count()))
                .ForMember(dest => dest.Dislikes, opt => opt.MapFrom(src => src.Likes.Where(x => x.Liked == false).Count()))
                .ForMember(dest => dest.LastPlayed, opt => opt.MapFrom(src => src.TrackPlays.Max(x => x.LastPlayed)));

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

            CreateMap<TrackPlay, MemberTrackDto>()
                .ForMember(dest => dest.YoutubeId, opt => opt.MapFrom(src => src.Track.YoutubeId))
                .ForMember(dest => dest.FirstPlayed, opt => opt.MapFrom(src => src.Track.CreatedOn))
                .ForMember(dest => dest.LastPlayed, opt => opt.MapFrom(src => src.Track.CreatedOn));

            // Mappings for TrackUserLike
            CreateMap<TrackLike, TrackUserLikeDto>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.DiscordId, opt => opt.MapFrom(src => src.User.DiscordId))
                .ForMember(dest => dest.Liked, opt => opt.MapFrom(src => src.Liked));
        }
    }
}