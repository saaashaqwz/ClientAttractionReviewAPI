using ClientAttractionReviewAPI.Services;
using ClientAttractionReviewAPI.ViewModels;
using Microsoft.Extensions.Logging;

namespace ClientAttractionReviewAPI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        RegisterPagesAndVM(builder.Services);
        return builder.Build();
    }
    
    private static void RegisterPagesAndVM(IServiceCollection service)
    {
        // services
        service.AddTransient<IAuthService, AuthService>();
        service.AddTransient<IAttractionService, AttractionService>();
        service.AddTransient<IReviewService, ReviewService>();

        // viewModels
        service.AddTransient<ReviewViewModel>();
            
        //pages
        service.AddTransient<MainPage>();
    }
}