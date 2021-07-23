using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs.Member;
using DisJockey.Core;

namespace API.Interfaces {
    public interface IUserRepository {
        Task<AppUser> GetUserByIdAsync(int id);
        Task<AppUser> GetUserByDiscordIdAsync(ulong discordId);
        Task<MemberDetailDto> GetMemberByDiscordIdAsync(ulong discordId);
        Task<IEnumerable<MemberListDto>> GetMembersAsync();
        void AddUser(AppUser user);
    }
}