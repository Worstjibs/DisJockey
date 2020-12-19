using API.DTOs;
using API.Entities;
using AutoMapper;

namespace API.Helpers {
    public class AutoMapperProfiles : Profile {
        public AutoMapperProfiles() {
            CreateMap<AppUser, MemberDto>();

            CreateMap<AppUser, UserDto>();

            CreateMap<Track, TrackDto>();

            CreateMap<Track, TrackUsersDto>()
                .ForMember(dest => dest.Users, opt => opt.MapFrom(src => src.AppUsers));
        }
    }
}