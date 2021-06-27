using API.Helpers;

namespace API.Interfaces {
    public interface IHaveCustomMapping {
        void CreateMapping(AutoMapperProfiles profile);
    }
}