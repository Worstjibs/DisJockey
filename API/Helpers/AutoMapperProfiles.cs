using System.Linq;
using API.DTOs;
using API.Entities;
using AutoMapper;

namespace API.Helpers {
    public class AutoMapperProfiles : Profile {
        public AutoMapperProfiles() {
            CreateMap<AppUser, MemberDto>();

            CreateMap<AppUser, UserDto>();

            CreateMap<Track, TrackDto>()
                .ForMember(dest => dest.Likes, opt => opt.MapFrom(src => src.Likes.Where(x => x.Liked == true).Count()))
                .ForMember(dest => dest.Dislikes, opt => opt.MapFrom(src => src.Likes.Where(x => x.Liked == false).Count()));

            CreateMap<Track, TrackUsersDto>()
                .ForMember(dest => dest.Users, opt => opt.MapFrom(src => src.AppUsers))
                .ForMember(dest => dest.Likes, opt => opt.MapFrom(src => src.Likes.Where(x => x.Liked == true).Count()))
                .ForMember(dest => dest.Dislikes, opt => opt.MapFrom(src => src.Likes.Where(x => x.Liked == false).Count()));
        }
    }
}