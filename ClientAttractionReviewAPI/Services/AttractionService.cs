using System.Text;
using System.Text.Json;
using ClientAttractionReviewAPI.Models.DTO;

namespace ClientAttractionReviewAPI.Services;

public class AttractionService : IAttractionService
{
    private readonly HttpClient _httpClient;
    private const string _baseUrl = "http://127.0.0.1:5282/api/Attractions"; //-----  проверьте свои адреса API

    public AttractionService()
    {
        _httpClient = new HttpClient();
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
    }

    public async Task<List<AttractionDTO>> GetAttractionsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync(_baseUrl);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var attractions = JsonSerializer.Deserialize<List<AttractionDTO>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Console.WriteLine($"Получено {attractions?.Count ?? 0} достопримечательностей");
            return attractions ?? new List<AttractionDTO>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка получения достопримечательностей: {ex.Message}");
            return new List<AttractionDTO>();
        }
    }

    public async Task<AttractionDTO?> GetAttractionAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/{id}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<AttractionDTO>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка получения достопримечательности: {ex.Message}");
            return null;
        }
    }

    public async Task<AttractionDTO?> CreateAttractionAsync(CreateAttractionDTO attraction, string token)
    {
        try
        {
            var json = JsonSerializer.Serialize(attraction);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PostAsync(_baseUrl, content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<AttractionDTO>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка создания достопримечательности: {ex.Message}");
            return null;
        }
        finally
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }

    public async Task<bool> DeleteAttractionAsync(int id, string token)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.DeleteAsync($"{_baseUrl}/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка удаления достопримечательности: {ex.Message}");
            return false;
        }
        finally
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }

    public async Task<AttractionDTO?> UpdateAttractionAsync(int id, CreateAttractionDTO attraction, string token)
    {
        try
        {
            var json = JsonSerializer.Serialize(attraction);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PutAsync($"{_baseUrl}/{id}", content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<AttractionDTO>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка обновления достопримечательности: {ex.Message}");
            return null;
        }
        finally
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
}