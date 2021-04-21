using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Models;
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

        public async Task<AppUser> GetUserByUsernameAsync(string username) {
            return await _context.Users
                .Include(x => x.Tracks)
                .FirstOrDefaultAsync(user => user.UserName == username);
        }

        public async Task<AppUser> GetUserByDiscordIdAsync(ulong discordId) {
            return await _context.Users
                .Include(x => x.Tracks)
                .FirstOrDefaultAsync(user => user.DiscordId == discordId);
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