using Microsoft.AspNetCore.Mvc;
using Orphanage.API.DTO_s.Auth;
using Orphanage.API.Services.Auth;

namespace Orphanage.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        private readonly IConfiguration _cfg;

        // Correct single constructor
        public AuthController(IAuthService auth, IConfiguration cfg)
        {
            _auth = auth;
            _cfg = cfg;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            await _auth.RegisterAsync(dto.Email, dto.Password, dto.FullName);
            return Ok();
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var (access, refresh) = await _auth.LoginAsync(dto.Email,
            dto.Password);
            return Ok(new { accessToken = access, refreshToken = refresh });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshDto dto)
        {
            var (access, refresh) = await _auth.RefreshAsync(dto.RefreshToken);
            return Ok(new { accessToken = access, refreshToken = refresh });
        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshDto dto)
        {
            await _auth.LogoutAsync(dto.RefreshToken);
            return Ok();
        }

        [HttpPost("google")]
        public async Task<IActionResult> Google([FromBody] IdTokenDto dto)
        {
            var (access, refresh) = await _auth.ExternalLoginGoogleAsync(dto.IdToken);
            return Ok(new { accessToken = access, refreshToken = refresh });
        }



    }
}
