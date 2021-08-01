using System.Threading.Tasks;
using DisJockey.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using AspNet.Security.OAuth.Discord;
using Microsoft.AspNetCore.Authentication.Cookies;
using DisJockey.Extensions;
using DisJockey.Shared.Extensions;

namespace DisJockey.Controllers {
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
            var discordId = User.GetDiscordId();

            if (discordId.HasValue) {
                var username = User.GetUsername();
                var avatarUrl = User.GetAvatarUrl();

                var userDto = new UserDto {
                    AvatarUrl = avatarUrl,
                    DiscordId = discordId.Value,
                    Username = username
                };

                return Ok(userDto);
            }

            return Ok(null);
        }
    }
}