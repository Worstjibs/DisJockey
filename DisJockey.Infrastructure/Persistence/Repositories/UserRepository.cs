using DisJockey.Shared.DTOs.Member;
using DisJockey.Core;
using Microsoft.EntityFrameworkCore;
using DisJockey.Services.Interfaces;
using DisJockey.Application.Mappers;

namespace DisJockey.Infrastructure.Persistence.Repositories;

public class UserRepository : BaseRepository, IUserRepository
{
    public UserRepository(DataContext context) : base(context) { }

    public async Task<AppUser?> GetUserByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<AppUser?> GetUserByDiscordIdAsync(ulong discordId)
    {
        return await _context.Users.AsQueryable()
            .Include(x => x.Playlists)
            .FirstOrDefaultAsync(x => x.DiscordId == discordId);
    }

    public async Task<MemberDetailDto?> GetMemberByDiscordIdAsync(ulong discordId)
    {
        var user = await _context.Users
            .Include(x => x.Tracks)
            .Include(x => x.Playlists)
            .FirstOrDefaultAsync(x => x.DiscordId == discordId);

        return user?.ToDetailDto();
    }

    public async Task<IEnumerable<MemberListDto>> GetMembersAsync()
    {
        var users = await _context.Users
            .Include(x => x.Tracks)
            .ToListAsync();

        return users.Select(u => u.ToListDto());
    }

    public void AddUser(AppUser user)
    {
        _context.Users.Add(user);
    }
}