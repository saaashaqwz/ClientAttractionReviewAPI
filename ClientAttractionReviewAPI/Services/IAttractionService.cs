using ClientAttractionReviewAPI.Models.DTO;

namespace ClientAttractionReviewAPI.Services;

public interface IAttractionService
{
    Task<List<AttractionDTO>> GetAttractionsAsync();
    Task<AttractionDTO?> GetAttractionAsync(int id);
    Task<AttractionDTO?> CreateAttractionAsync(CreateAttractionDTO attraction, string token);
    Task<AttractionDTO?> UpdateAttractionAsync(int id, CreateAttractionDTO attraction, string token);
    Task<bool> DeleteAttractionAsync(int id, string token);
}