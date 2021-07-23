using System;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using DisJockey.Core;
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

                var userDto = new UserDto {
                    AvatarUrl = avatarUrl,
                    DiscordId = discordIdStr,
                    Username = username
                };

                return Ok(userDto);
            }

            return Ok(null);
        }
    }
}