
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Generators;
using Orphanage.API.Services.Jwt;
using Orphanage.Core.Models;
using Orphanage.Infrastructure.Data;

namespace Orphanage.API.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _db;
        private readonly IJwtService _jwt;
        private readonly IConfiguration _cfg;

        public AuthService(ApplicationDbContext db, IJwtService jwt, IConfiguration cfg)
        {
            _db = db;
            _jwt = jwt;
            _cfg = cfg;
        }

        public async Task RegisterAsync(string email, string password, string fullName)
        {
            if (await _db.Users.AnyAsync(u => u.Email == email))
                throw new InvalidOperationException("Email already registered");
            var user = new User
            {
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                FullName = fullName
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }

        public async Task<(string accessToken, string refreshToken)> LoginAsync(string email, string password)
        {
            var user = await _db.Users.Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role).FirstOrDefaultAsync(u => u.Email == email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password,
            user.PasswordHash))
                throw new InvalidOperationException("Invalid credentials");

            var roles = user.UserRoles.Select(ur => ur.Role.Name);
            var access = _jwt.GenerateAccessToken(user, roles);
            var refresh = _jwt.GenerateRefreshToken();
            var refreshEntity = new RefreshToken
            {
                Token = refresh,
                Expires =
            DateTime.UtcNow.AddDays(int.Parse(_cfg["Jwt:RefreshTokenDays"] ?? "30")),
                UserId = user.Id
            };
            _db.RefreshTokens.Add(refreshEntity);
            await _db.SaveChangesAsync();
            return (access, refresh);
        }

        public async Task<(string accessToken, string refreshToken)> RefreshAsync(string refreshToken)
        {
            var ent = await _db.RefreshTokens.Include(r => r.User)
                .ThenInclude(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(r => r.Token == refreshToken && !r.IsRevoked);

            if (ent == null || ent.Expires < DateTime.UtcNow) throw new
            InvalidOperationException("Invalid refresh token");
            // revoke old
            ent.IsRevoked = true;
            var user = ent.User;
            var roles = user.UserRoles.Select(ur => ur.Role.Name);
            var access = _jwt.GenerateAccessToken(user, roles);
            var newRefresh = _jwt.GenerateRefreshToken();
            var newEnt = new RefreshToken
            {
                Token = newRefresh,
                Expires =
            DateTime.UtcNow.AddDays(int.Parse(_cfg["Jwt:RefreshTokenDays"] ?? "30")),
                UserId = user.Id
            };
            _db.RefreshTokens.Add(newEnt);
            await _db.SaveChangesAsync();
            return (access, newRefresh);
        }

        public async Task LogoutAsync(string refreshToken)
        {
            var ent = await _db.RefreshTokens.FirstOrDefaultAsync(r => r.Token == refreshToken);
            if (ent == null) return;
            ent.IsRevoked = true;
            await _db.SaveChangesAsync();
        }

        public async Task<(string accessToken, string refreshToken)>ExternalLoginGoogleAsync(string idToken)
        {
            // Verify token with Google
            var settings = new
            Google.Apis.Auth.GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new[] { _cfg["Google:ClientId"] }
            };
            var payload = await
            Google.Apis.Auth.GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            var email = payload.Email;
            var user = await _db.Users.Include(u => u.UserRoles).ThenInclude(ur
            => ur.Role).FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                user = new User
                {
                    Email = email,
                    FullName = payload.Name ?? payload.Email,
                    PasswordHash = string.Empty
                };
                _db.Users.Add(user);
                await _db.SaveChangesAsync();
            }
            var roles = user.UserRoles.Select(ur => ur.Role.Name);
            var access = _jwt.GenerateAccessToken(user, roles);
            var refresh = _jwt.GenerateRefreshToken();
            _db.RefreshTokens.Add(new RefreshToken
            {
                Token = refresh,
                Expires = DateTime.UtcNow.AddDays(int.Parse(_cfg["Jwt:RefreshTokenDays"] ?? "30")),
                UserId = user.Id
            });
            await _db.SaveChangesAsync();
            return (access, refresh);
        }




    }
}
