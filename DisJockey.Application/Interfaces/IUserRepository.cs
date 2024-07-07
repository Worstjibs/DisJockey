using System.Collections.Generic;
using System.Threading.Tasks;
using DisJockey.Shared.DTOs.Member;
using DisJockey.Core;

namespace DisJockey.Services.Interfaces {
    public interface IUserRepository {
        Task<AppUser> GetUserByIdAsync(int id);
        Task<AppUser> GetUserByDiscordIdAsync(ulong discordId);
        Task<MemberDetailDto> GetMemberByDiscordIdAsync(ulong discordId);
        Task<IEnumerable<MemberListDto>> GetMembersAsync();
        void AddUser(AppUser user);
    }
}