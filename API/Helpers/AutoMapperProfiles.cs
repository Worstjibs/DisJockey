using System.Linq;
using API.DTOs;
using API.Models;
using AutoMapper;

namespace API.Helpers {
    public class AutoMapperProfiles : Profile {
        public AutoMapperProfiles() {
            CreateMap<AppUser, MemberDto>()
                .ForMember(dest => dest.DateJoined, opt => opt.MapFrom(src => src.CreatedOn));

            CreateMap<AppUser, TrackUserDto>();

            CreateMap<Track, MemberTrackDto>();
                // .ForMember(dest => dest.Likes, opt => opt.MapFrom(src => src.Likes.Where(x => x.Liked == true).Count()))
                // .ForMember(dest => dest.Dislikes, opt => opt.MapFrom(src => src.Likes.Where(x => x.Liked == false).Count()));

            CreateMap<Track, TrackDto>()
                .ForMember(dest => dest.Users, opt => opt.MapFrom(src => src.AppUsers))
                    // .GroupBy(x => new {x.User.DiscordId, x.User.UserName}).Select(t => new TrackUserDto {
                    //     CreatedOn = t.Min(x => x.CreatedOn),
                    //     LastPlayed =  t.Max(x => x.CreatedOn),
                    //     TimesPlayed = t.Count(),
                    //     DiscordId = t.Key.DiscordId,
                    //     Username = t.Key.UserName    
                    // })))
                .ForMember(dest => dest.UserLikes, opt => opt.MapFrom(src => src.Likes))
                .ForMember(dest => dest.Likes, opt => opt.MapFrom(src => src.Likes.Where(x => x.Liked == true).Count()))
                .ForMember(dest => dest.Dislikes, opt => opt.MapFrom(src => src.Likes.Where(x => x.Liked == false).Count()));

            // Mappings for AppUserTrack
            CreateMap<AppUserTrack, TrackUserDto>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.DiscordId, opt => opt.MapFrom(src => src.User.DiscordId));

            CreateMap<AppUserTrack, MemberTrackDto>()
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