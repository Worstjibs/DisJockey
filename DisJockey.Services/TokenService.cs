using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DisJockey.Core;
using Microsoft.IdentityModel.Tokens;
using DisJockey.Shared.Helpers;
using DisJockey.Services.Interfaces;

namespace DisJockey.Services {
    public class TokenService : ITokenService {
        private readonly SymmetricSecurityKey _key;
        public TokenService(AuthenticationSettings settings) {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.JwtTokenKey));
        }

        public string CreateTokenAsync(AppUser user) {
            var claims = new List<Claim> {
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
            };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}