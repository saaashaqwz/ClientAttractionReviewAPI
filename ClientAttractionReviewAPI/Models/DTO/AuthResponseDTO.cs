using System.Text.Json.Serialization;

namespace ClientAttractionReviewAPI.Models.DTO;

public class AuthResponseDTO
{
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;
    [JsonPropertyName("refreshToken")]
    public string RefreshToken { get; set; } = string.Empty;
    [JsonPropertyName("validTo")]
    public DateTime ValidTo { get; set; }
    public UserDTO User { get; set; } = new UserDTO();
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}