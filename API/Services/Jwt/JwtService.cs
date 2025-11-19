using Microsoft.IdentityModel.Tokens;
using Orphanage.Core.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Orphanage.API.Services.Jwt
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _cfg;
        public JwtService(IConfiguration cfg) => _cfg = cfg;
        public string GenerateAccessToken(User user, IEnumerable<string> roles)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]));

            var creds = new SigningCredentials(key,
            SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
                 {
                  new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                  new Claim(JwtRegisteredClaimNames.Email, user.Email)
                };
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));
            var token = new JwtSecurityToken(
            issuer: _cfg["Jwt:Issuer"],
            audience: _cfg["Jwt:Audience"],
            claims: claims,
            expires:
            DateTime.UtcNow.AddMinutes(int.Parse(_cfg["Jwt:AccessTokenMinutes"] ??
            "15")),
            signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

    }
}
