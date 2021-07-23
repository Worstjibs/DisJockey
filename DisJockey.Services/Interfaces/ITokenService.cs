using System.Threading.Tasks;
using DisJockey.Core;

namespace DisJockey.Services.Interfaces {
    public interface ITokenService {
        string CreateTokenAsync(AppUser user);
    }
}