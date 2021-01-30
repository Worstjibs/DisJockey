using System.Linq;
using API.DTOs;
using API.Entities;
using AutoMapper;

namespace API.Helpers {
    public class AutoMapperProfiles : Profile {
        public AutoMapperProfiles() {
            CreateMap<AppUser, MemberDto>();

            CreateMap<AppUser, TrackUserDto>();

            CreateMap<Track, MemberTrackDto>()
                .ForMember(dest => dest.Likes, opt => opt.MapFrom(src => src.Likes.Where(x => x.Liked == true).Count()))
                .ForMember(dest => dest.Dislikes, opt => opt.MapFrom(src => src.Likes.Where(x => x.Liked == false).Count()));

            CreateMap<Track, TrackDto>()
                .ForMember(dest => dest.Users, opt => opt.MapFrom(src => src.AppUsers))
                .ForMember(dest => dest.UserLikes, opt => opt.MapFrom(src => src.Likes))
                .ForMember(dest => dest.Likes, opt => opt.MapFrom(src => src.Likes.Where(x => x.Liked == true).Count()))
                .ForMember(dest => dest.Dislikes, opt => opt.MapFrom(src => src.Likes.Where(x => x.Liked == false).Count()));

            // Mappings for AppUserTrack
            CreateMap<AppUserTrack, TrackUserDto>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.DiscordId, opt => opt.MapFrom(src => src.User.DiscordId));

            CreateMap<AppUserTrack, MemberTrackDto>()
                .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => src.Track.CreatedOn))
                .ForMember(dest => dest.YoutubeId, opt => opt.MapFrom(src => src.Track.YoutubeId));

            // Mappings for TrackUserLike
            CreateMap<TrackLike, TrackUserLikeDto>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.DiscordId, opt => opt.MapFrom(src => src.User.DiscordId))
                .ForMember(dest => dest.Liked, opt => opt.MapFrom(src => src.Liked));
        }
    }
}