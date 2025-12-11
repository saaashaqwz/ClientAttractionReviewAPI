using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ClientAttractionReviewAPI.Models.DTO;

public class UserDTO
{
    [JsonPropertyName("id")]
    [Key]
    public int Id { get; set; }
    
    [JsonPropertyName("username")]
    [Required (ErrorMessage = "Имя пользователя не может быть пустым")]
    [StringLength(50, ErrorMessage = "Имя пользователя не может содержать больше 50 символов")]
    public string Username { get; set; }
    
    [JsonPropertyName("email")]
    [Required (ErrorMessage = "Email не может быть пустым")]
    [EmailAddress]
    [StringLength(100, ErrorMessage = "Email не может содержать больше 100 символов")]
    public string Email { get; set; }
    
    [JsonPropertyName("roleName")]
    public string RoleName  { get; set; }
}