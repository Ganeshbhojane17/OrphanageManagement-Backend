using Orphanage.Core.Models;

namespace Orphanage.API.Services.Jwt
{
    public interface IJwtService
    {
        string GenerateAccessToken(User user, IEnumerable<string> roles);
        string GenerateRefreshToken();
    }
}
