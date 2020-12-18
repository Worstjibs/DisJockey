using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers {
    public class AccountController : BaseApiController {
        private readonly DataContext _context;
        public AccountController(DataContext context) {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto) {
            // Check the user already exists in the DbContext
            if (await CheckUserExists(registerDto.Username)) return BadRequest("Username is taken");

            // Generate a new AppUser
            AppUser user = new AppUser
            {
                UserName = registerDto.Username.ToLower()
            };

            // Add it to the DbContext
            _context.Users.Add(user);

            // Save changes asynchronously
            await _context.SaveChangesAsync();

            // Return a new UserDto as the response
            return new UserDto
            {
                Username = user.UserName
            };
        }

        public async Task<bool> CheckUserExists(string username) {
            // Query the DbContext to see if a user exists with the same username
            return await _context.Users.AnyAsync(user => user.UserName == username.ToLower());
        }
    }
}