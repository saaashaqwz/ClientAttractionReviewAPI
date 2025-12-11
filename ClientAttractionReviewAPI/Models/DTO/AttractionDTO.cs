using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ClientAttractionReviewAPI.Models.DTO;

public class AttractionDTO
{
    [JsonPropertyName("id")]
    [Key]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    [Required (ErrorMessage = "название не может быть пустым")]
    [StringLength(50, ErrorMessage = "название не может содержать больше 50 символов")]
    public string Name { get; set; } = null!;
    
    [JsonPropertyName("description")]
    [StringLength(100, ErrorMessage = "описание не может содержать больше 100 символов")]
    public string? Description { get; set; } 
    
    [JsonPropertyName("location")]
    [Required (ErrorMessage = "место не может быть пустым")]
    [StringLength(50, ErrorMessage = "место не может содержать больше 50 символов")]
    public string Location { get; set; } = null!; 
    
    [JsonPropertyName("category")]
    [StringLength(40, ErrorMessage = "категория не может содержать больше 40 символов")]
    public string? Category { get; set; }
}