using DisJockey.Shared.DTOs.Member;
using DisJockey.Core;
using DisJockey.Application.Interfaces;

namespace DisJockey.Services.Interfaces;

public interface IUserRepository : IBaseRepository
{
    Task<AppUser?> GetUserByIdAsync(int id);
    Task<AppUser?> GetUserByDiscordIdAsync(ulong discordId);
    Task<MemberDetailDto?> GetMemberByDiscordIdAsync(ulong discordId);
    Task<IEnumerable<MemberListDto>> GetMembersAsync();
    void AddUser(AppUser user);
}