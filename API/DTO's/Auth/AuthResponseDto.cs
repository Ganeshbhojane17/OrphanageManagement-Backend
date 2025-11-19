namespace Orphanage.API.DTO_s.Auth
{
    public class AuthResponseDto
    {
        public string AccessToken { get; set; } = string.Empty!;
        public string RefreshToken { get; set; } = string.Empty!;
        public string Email { get; set; } = string.Empty!;
        public string? FullName { get; set; }
    }
}
