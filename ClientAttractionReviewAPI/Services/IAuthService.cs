using ClientAttractionReviewAPI.Models.DTO;

namespace ClientAttractionReviewAPI.Services;

public interface IAuthService
{
    Task<AuthResponseDTO?> RegisterAsync(RegisterRequestDTO registerRequest);
    Task<AuthResponseDTO?> LoginAsync(LoginRequestDTO loginRequest);
    Task<AuthResponseDTO?> RefreshTokenAsync(string refreshToken);
    
    Task<bool> LoadAuthData();
    
    bool IsAuthenticated { get; }
    string Token { get; }
    UserDTO? CurrentUser { get; }
    void Logout();
}