using ClientAttractionReviewAPI.Models.DTO;

namespace ClientAttractionReviewAPI.Services;

public interface IReviewService
{
    Task<List<ReviewDTO>> GetReviewsAsync();
    Task<ReviewDTO?> GetReviewAsync(int id);
    Task<List<ReviewDTO>> GetReviewsByAttractionAsync(int attractionId);
    Task<ReviewDTO?> CreateReviewAsync(CreateReviewDTO review, string token);
    Task<ReviewDTO?> UpdateReviewAsync(int id, UpdateReviewDTO review, string token);
    Task<bool> DeleteReviewAsync(int id, string token);
}