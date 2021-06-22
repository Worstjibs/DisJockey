using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;

namespace API.Interfaces {
    public interface IUserRepository {
        Task<AppUser> GetUserByIdAsync(int id);
        Task<AppUser> GetUserByUsernameAsync(string username);
        Task<AppUser> GetUserByDiscordIdAsync(ulong discordId);
        Task<IEnumerable<MemberDto>> GetMembersAsync();
        void AddUser(AppUser user);
    }
}