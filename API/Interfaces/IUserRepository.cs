using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs.Member;
using API.Entities;

namespace API.Interfaces {
    public interface IUserRepository {
        Task<AppUser> GetUserByIdAsync(int id);
        Task<AppUser> GetUserByDiscordIdAsync(ulong discordId);
        Task<MemberDto> GetMemberByDiscordIdAsync(ulong discordId);
        Task<IEnumerable<MemberDto>> GetMembersAsync();
        void AddUser(AppUser user);
    }
}