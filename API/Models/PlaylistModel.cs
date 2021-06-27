using API.Entities;
using API.Helpers;
using API.Interfaces;

namespace API.Models {
    public class PlaylistModel : IHaveCustomMapping {
        public string Name { get; set; }
        public void CreateMapping(AutoMapperProfiles profile) {
            profile.CreateMap<Playlist, PlaylistModel>()
                .ForMember(x => x.Name, opt => opt.MapFrom(dest => dest.Name));
        }
    }
}