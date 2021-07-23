using System.Threading.Tasks;
using DisJockey.Core;

namespace API.Interfaces {
    public interface ITokenService {
        string CreateTokenAsync(AppUser user);
    }
}