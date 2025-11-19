namespace Orphanage.API.Services.Auth
{
    public interface IAuthService
    {
        Task<(string accessToken, string refreshToken)> LoginAsync(string email, string password);
        Task RegisterAsync(string email, string password, string fullName);
        Task<(string accessToken, string refreshToken)> RefreshAsync(string refreshToken);
        Task LogoutAsync(string refreshToken);
        Task<(string accessToken, string refreshToken)>
        ExternalLoginGoogleAsync(string idToken);
    }
}
