using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.Member;
using API.Entities;
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
            return await _context.Users.AsQueryable().FirstOrDefaultAsync(x => x.DiscordId == discordId);
        }

        public async Task<MemberDto> GetMemberByDiscordIdAsync(ulong discordId) {
            return await _context.Users
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(user => user.DiscordId == discordId.ToString());
        }

        public async Task<IEnumerable<MemberDto>> GetMembersAsync() {
            return await _context.Users
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public void AddUser(AppUser user) {
            _context.Users.Add(user);
        }
    }
}