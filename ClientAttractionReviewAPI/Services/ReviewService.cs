using System.Text;
using System.Text.Json;
using ClientAttractionReviewAPI.Models.DTO;

namespace ClientAttractionReviewAPI.Services;

public class ReviewService : IReviewService
    {
        private readonly HttpClient _httpClient;
        private const string _baseUrl = "http://127.0.0.1:5282/api/Reviews"; //-----  проверьте свои адреса API

        public ReviewService()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        public async Task<List<ReviewDTO>> GetReviewsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(_baseUrl);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var reviews = JsonSerializer.Deserialize<List<ReviewDTO>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                Console.WriteLine($"Получено {reviews?.Count ?? 0} отзывов");
                return reviews ?? new List<ReviewDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка получения отзывов: {ex.Message}");
                return new List<ReviewDTO>();
            }
        }

        public async Task<ReviewDTO?> GetReviewAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/{id}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ReviewDTO>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка получения отзыва: {ex.Message}");
                return null;
            }
        }

        public async Task<List<ReviewDTO>> GetReviewsByAttractionAsync(int attractionId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/attraction/{attractionId}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<ReviewDTO>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<ReviewDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка получения отзывов по достопримечательности: {ex.Message}");
                return new List<ReviewDTO>();
            }
        }

        public async Task<ReviewDTO?> CreateReviewAsync(CreateReviewDTO review, string token)
        {
            try
            {
                var json = JsonSerializer.Serialize(review);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.PostAsync(_baseUrl, content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ReviewDTO>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка создания отзыва: {ex.Message}");
                return null;
            }
            finally
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        public async Task<bool> DeleteReviewAsync(int id, string token)
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
                Console.WriteLine($"Ошибка удаления отзыва: {ex.Message}");
                return false;
            }
            finally
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        public async Task<ReviewDTO?> UpdateReviewAsync(int id, UpdateReviewDTO review, string token)
        {
            try
            {
                var json = JsonSerializer.Serialize(review);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.PutAsync($"{_baseUrl}/{id}", content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ReviewDTO>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка обновления отзыва: {ex.Message}");
                return null;
            }
            finally
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }
    }