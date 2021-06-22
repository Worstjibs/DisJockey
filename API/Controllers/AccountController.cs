using System;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using AspNet.Security.OAuth.Discord;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using API.Extensions;

namespace API.Controllers {
    public class AccountController : BaseApiController {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        public AccountController(DataContext context, ITokenService tokenService) {
            _tokenService = tokenService;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterDto registerDto) {
            var discordId = registerDto.DiscordId;

            // Check if the Discor duser is already registered
            if (await CheckDiscordUserExists(discordId)) return Conflict("Discord user already registered");

            // Check the user already exists in the DbContext
            if (await CheckUserExists(registerDto.Username)) return Conflict("Username is taken");

            // Generate a new AppUser
            AppUser user = new AppUser {
                UserName = registerDto.Username.ToLower(),
                CreatedOn = DateTime.Now,
                DiscordId = discordId
            };

            // Add it to the DbContext
            _context.Users.Add(user);

            // Save changes asynchronously
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("login")]
        public ActionResult Login() {
            var redirectUri = "/tracks";

            // if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development") redirectUri = "https://localhost:4200" + redirectUri;

            var challenge = Challenge(new AuthenticationProperties { RedirectUri = redirectUri }, DiscordAuthenticationDefaults.AuthenticationScheme );
            return challenge;
        }

        [HttpGet("logout")]
        public async Task<ActionResult> Logout() {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Redirect("/");
        }
        
        [HttpGet("claims")]
        public ActionResult<UserDto> GetUserInfo() {
            var discordIdStr = User.GetDiscordId();

            if (discordIdStr != null) {
                var username = User.GetUsername();
                var avatarUrl = User.GetAvatarUrl();

                var identity = User.Identity;

                ulong discordId;
                UInt64.TryParse(discordIdStr, out discordId);

                var userDto = new UserDto {
                    AvatarUrl = avatarUrl,
                    DiscordId = discordId,
                    Username = username
                };

                return Ok(userDto);
            }

            return Ok(null);
        }

        private async Task<bool> CheckUserExists(string username) {
            // Query the DbContext to see if a user exists with the same username
            return await _context.Users.AnyAsync(user => user.UserName == username.ToLower());
        }

        private async Task<bool> CheckDiscordUserExists(ulong discordId) {
            // Query the DbContext to see if a user exists with the same DiscordId
            return await _context.Users.AnyAsync(user => user.DiscordId == discordId);
        }
    }
}