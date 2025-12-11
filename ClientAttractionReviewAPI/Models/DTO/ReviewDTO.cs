using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ClientAttractionReviewAPI.Models.DTO;

public class ReviewDTO
{
    [JsonPropertyName("id")]
    [Key]
    public int Id { get; set; }
    
    [JsonPropertyName("title")]
    [Required (ErrorMessage = "заголовок не может быть пустым")]
    [StringLength(30, ErrorMessage = "заголовок не может содержать больше 30 символов")]
    public string Title { get; set; } = null!;
    
    [JsonPropertyName("content")]
    [Required (ErrorMessage = "отзыв не может быть пустым")]
    [MinLength(20, ErrorMessage = "отзыв должен содержать от 20 символов")]
    [StringLength(500, ErrorMessage = "отзыв не может содержать больше 500 символов")]
    public string Content { get; set; } = null!;
    
    [JsonPropertyName("rating")]
    [Required (ErrorMessage = "отзыв не может быть без оценки")]
    [Range(1, 5, ErrorMessage = "оценка должна быть от 1 до 5")]
    public int Rating { get; set; } 
    
    [JsonPropertyName("author")]
    [Required (ErrorMessage = "автор не может быть пустым")]
    [MinLength(10, ErrorMessage = "автор должен содержать от 10 символов")]
    public string Author { get; set; } = null!;
    
    [JsonPropertyName("createdDate")]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    [JsonPropertyName("attractionId")]
    public int AttractionId { get; set; }
}