using System.Threading.Tasks;
using API.Models;

namespace API.Interfaces {
    public interface ITokenService {
        string CreateTokenAsync(AppUser user);
    }
}