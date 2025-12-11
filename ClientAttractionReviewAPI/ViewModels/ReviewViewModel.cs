using System.Collections.ObjectModel;
using ClientAttractionReviewAPI.Models.DTO;
using ClientAttractionReviewAPI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ClientAttractionReviewAPI.ViewModels;

public partial class ReviewViewModel : ObservableObject
{
    private readonly IAuthService _authService;
    private readonly IAttractionService _attractionService;
    private readonly IReviewService _reviewService;

    [ObservableProperty]
    private ObservableCollection<AttractionDTO> _attractions = new();

    [ObservableProperty]
    private ObservableCollection<ReviewDTO> _reviews = new();

    [ObservableProperty]
    private string _username = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private bool _isLoggedIn = false;

    [ObservableProperty]
    private UserDTO? _currentUser;

    [ObservableProperty]
    private bool _isLoading = false;

    public ReviewViewModel(IAuthService authService, IAttractionService attractionService, IReviewService reviewService)
    {
        _authService = authService;
        _attractionService = attractionService;
        _reviewService = reviewService;
        
        // Проверяем, есть ли сохраненная сессия
        CheckLoginStatus();
    }

    private async void CheckLoginStatus()
    {
        IsLoading = true;
        await _authService.LoadAuthData();
        
        IsLoggedIn = _authService.IsAuthenticated;
        CurrentUser = _authService.CurrentUser;
        
        if (IsLoggedIn)
        {
            await LoadDataAsync();
        }
        
        IsLoading = false;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        try
        {
            IsLoading = true;
            
            var loginRequest = new LoginRequestDTO
            {
                EmailOrUsername = Username,
                Password = Password,
                RoleId = 2 // User role
            };

            var result = await _authService.LoginAsync(loginRequest);
            
            if (result?.Success == true)
            {
                IsLoggedIn = true;
                CurrentUser = result.User;
                await LoadDataAsync();
                Console.WriteLine($"Вход выполнен! Пользователь: {CurrentUser.Username}");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", 
                    result?.ErrorMessage ?? "Не удалось войти", "OK");
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Ошибка", 
                $"Ошибка входа: {ex.Message}", "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task RegisterAsync()
    {
        try
        {
            IsLoading = true;
            
            var registerRequest = new RegisterRequestDTO
            {
                Username = Username,
                Email = $"{Username}@example.com", // В реальном приложении нужно поле для email
                Password = Password,
                RoleId = 2
            };

            var result = await _authService.RegisterAsync(registerRequest);
            
            if (result?.Success == true)
            {
                await Application.Current.MainPage.DisplayAlert("Успех", 
                    "Регистрация прошла успешно! Теперь войдите в систему.", "OK");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", 
                    result?.ErrorMessage ?? "Не удалось зарегистрироваться", "OK");
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Ошибка", 
                $"Ошибка регистрации: {ex.Message}", "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void Logout()
    {
        _authService.Logout();
        IsLoggedIn = false;
        CurrentUser = null;
        Attractions.Clear();
        Reviews.Clear();
        
        Username = string.Empty;
        Password = string.Empty;
    }

    [RelayCommand]
    private async Task LoadAttractionsAsync()
    {
        try
        {
            IsLoading = true;
            var attractions = await _attractionService.GetAttractionsAsync();
            
            Attractions.Clear();
            foreach (var attraction in attractions)
            {
                Attractions.Add(attraction);
                Console.WriteLine($"Достопримечательность: {attraction.Name}, {attraction.Location}");
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Ошибка", 
                $"Ошибка загрузки достопримечательностей: {ex.Message}", "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task LoadReviewsAsync()
    {
        try
        {
            IsLoading = true;
            var reviews = await _reviewService.GetReviewsAsync();
            
            Reviews.Clear();
            foreach (var review in reviews)
            {
                Reviews.Add(review);
                Console.WriteLine($"Отзыв: {review.Title}, Оценка: {review.Rating}");
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Ошибка", 
                $"Ошибка загрузки отзывов: {ex.Message}", "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadDataAsync()
    {
        await LoadAttractionsAsync();
        await LoadReviewsAsync();
    }

    [RelayCommand]
    private async Task CreateSampleAttractionAsync()
    {
        if (!IsLoggedIn)
        {
            await Application.Current.MainPage.DisplayAlert("Ошибка", 
                "Требуется авторизация", "OK");
            return;
        }

        try
        {
            var attraction = new CreateAttractionDTO
            {
                Name = "Тестовая достопримечательность",
                Description = "Создано из MAUI приложения",
                Location = "Москва",
                Category = "Тест"
            };

            var result = await _attractionService.CreateAttractionAsync(attraction, _authService.Token);
            
            if (result != null)
            {
                await Application.Current.MainPage.DisplayAlert("Успех", 
                    $"Создана достопримечательность: {result.Name}", "OK");
                await LoadAttractionsAsync();
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Ошибка", 
                $"Ошибка создания: {ex.Message}", "OK");
        }
    }
}