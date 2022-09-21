using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OBPostupyApi.Entities;
using OBPostupyApi.Settings;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OBPostupyApi.Services
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<User> _userManager;
        private readonly JwtSettings _settings;

        public TokenService(UserManager<User> userManager, IOptions<JwtSettings> options)
        {
            _userManager = userManager;
            _settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<string> GenerateTokenAsync(User user, DateTime expiration)
        {
            var claims = new List<Claim>
            {
                 new Claim(ClaimTypes.NameIdentifier, user.Id),
                 new Claim("firstName", user.FirstName),
                 new Claim("lastName", user.LastName),
                 new Claim(JwtRegisteredClaimNames.Email, user.Email),
                 new Claim("expiration", expiration.ToString("o")),
                 new Claim(JwtRegisteredClaimNames.Sub,user.Email),
                 new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            //Roles
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, role));
            }

            // Create the JWT security token and encode it.
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var jwt = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: expiration,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}
