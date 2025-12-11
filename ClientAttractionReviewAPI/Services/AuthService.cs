using System.Text;
using System.Text.Json;
using ClientAttractionReviewAPI.Models.DTO;

namespace ClientAttractionReviewAPI.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private const string _baseUrl = "http://127.0.0.1:5282/api/Auth"; //-----  проверьте свои адреса API
    private AuthResponseDTO? _currentAuth;

    public bool IsAuthenticated => _currentAuth != null && 
                                  !string.IsNullOrEmpty(_currentAuth.Token) &&
                                  _currentAuth.ValidTo > DateTime.UtcNow;

    public string Token => _currentAuth?.Token ?? string.Empty;
    public UserDTO? CurrentUser => _currentAuth?.User;

    public AuthService()
    {
        _httpClient = new HttpClient();
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
    }

    public async Task<AuthResponseDTO?> RegisterAsync(RegisterRequestDTO registerRequest)
    {
        try
        {
            var json = JsonSerializer.Serialize(registerRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_baseUrl}/Register", content);
            
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var authResponse = JsonSerializer.Deserialize<AuthResponseDTO>(responseContent);
                
                if (authResponse?.Success == true)
                {
                    _currentAuth = authResponse;
                    await SaveAuthData();
                }
                
                return authResponse;
            }
            
            return new AuthResponseDTO
            {
                Success = false,
                ErrorMessage = $"Ошибка регистрации: {response.StatusCode}"
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка регистрации: {ex.Message}");
            return new AuthResponseDTO
            {
                Success = false,
                ErrorMessage = $"Ошибка соединения: {ex.Message}"
            };
        }
    }

    public async Task<AuthResponseDTO?> LoginAsync(LoginRequestDTO loginRequest)
    {
        try
        {
            var json = JsonSerializer.Serialize(loginRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_baseUrl}/Login", content);
            
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var authResponse = JsonSerializer.Deserialize<AuthResponseDTO>(responseContent);
                
                if (authResponse?.Success == true)
                {
                    _currentAuth = authResponse;
                    await SaveAuthData();
                    Console.WriteLine($"Успешный вход! Токен: {authResponse.Token}");
                }
                
                return authResponse;
            }
            
            return new AuthResponseDTO
            {
                Success = false,
                ErrorMessage = $"Ошибка входа: {response.StatusCode}"
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка входа: {ex.Message}");
            return new AuthResponseDTO
            {
                Success = false,
                ErrorMessage = $"Ошибка соединения: {ex.Message}"
            };
        }
    }

    public async Task<AuthResponseDTO?> RefreshTokenAsync(string refreshToken)
    {
        try
        {
            var response = await _httpClient.PostAsync(
                $"{_baseUrl}/Refresh?refreshToken={Uri.EscapeDataString(refreshToken)}", 
                null);
            
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var authResponse = JsonSerializer.Deserialize<AuthResponseDTO>(responseContent);
                
                if (authResponse?.Success == true)
                {
                    _currentAuth = authResponse;
                    await SaveAuthData();
                }
                
                return authResponse;
            }
            
            return new AuthResponseDTO
            {
                Success = false,
                ErrorMessage = "Не удалось обновить токен"
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка обновления токена: {ex.Message}");
            return null;
        }
    }

    private async Task SaveAuthData()
    {
        if (_currentAuth != null)
        {
            await SecureStorage.SetAsync("auth_token", _currentAuth.Token);
            await SecureStorage.SetAsync("refresh_token", _currentAuth.RefreshToken);
            
            var userJson = JsonSerializer.Serialize(_currentAuth.User);
            await SecureStorage.SetAsync("current_user", userJson);
            
            var validTo = _currentAuth.ValidTo.ToString("O");
            await SecureStorage.SetAsync("token_valid_to", validTo);
        }
    }

    public async Task<bool> LoadAuthData()
    {
        try
        {
            var token = await SecureStorage.GetAsync("auth_token");
            var refreshToken = await SecureStorage.GetAsync("refresh_token");
            var userJson = await SecureStorage.GetAsync("current_user");
            var validToStr = await SecureStorage.GetAsync("token_valid_to");

            if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(userJson))
            {
                var user = JsonSerializer.Deserialize<UserDTO>(userJson);
                var validTo = DateTime.Parse(validToStr);

                _currentAuth = new AuthResponseDTO
                {
                    Token = token,
                    RefreshToken = refreshToken ?? string.Empty,
                    ValidTo = validTo,
                    User = user!,
                    Success = true
                };

                if (validTo < DateTime.UtcNow.AddMinutes(5) && !string.IsNullOrEmpty(refreshToken))
                {
                    var refreshResult = await RefreshTokenAsync(refreshToken);
                    return refreshResult?.Success == true;
                }

                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка загрузки данных аутентификации: {ex.Message}");
        }

        return false;
    }

    public void Logout()
    {
        _currentAuth = null;
        SecureStorage.Remove("auth_token");
        SecureStorage.Remove("refresh_token");
        SecureStorage.Remove("current_user");
        SecureStorage.Remove("token_valid_to");
    }
}