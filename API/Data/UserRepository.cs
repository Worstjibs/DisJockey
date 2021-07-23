using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.Member;
using DisJockey.Core;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data {
    public class UserRepository : IUserRepository {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public UserRepository(DataContext context, IMapper mapper) {
            _mapper = mapper;
            _context = context;
        }

        public async Task<AppUser> GetUserByIdAsync(int id) {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByDiscordIdAsync(ulong discordId) {
            return await _context.Users.AsQueryable()
                .Include(x => x.Playlists)
                .FirstOrDefaultAsync(x => x.DiscordId == discordId);
        }

        public async Task<MemberDetailDto> GetMemberByDiscordIdAsync(ulong discordId) {
            return await _context.Users.AsQueryable()
                .Where(x => x.DiscordId == discordId)
                .ProjectTo<MemberDetailDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<MemberListDto>> GetMembersAsync() {
            return await _context.Users
                .ProjectTo<MemberListDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public void AddUser(AppUser user) {
            _context.Users.Add(user);
        }
    }
}