using KartinMauiApp_Karting_OCRgApp.ViewModels;
using MauiApp_Karting_OCR.Pages;
using MauiApp_Karting_OCR.ViewModels;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using MauiApp_Karting_OCR.Services;

namespace MauiApp_Karting_OCR
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder.UseMauiApp<App>().ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            }).UseMauiCommunityToolkit();
#if DEBUG
            builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<CircuitService>();
            builder.Services.AddSingleton<LapTimeService>();
            builder.Services.AddSingleton<OcrService>();

            builder.Services.AddSingleton<OcrScanViewModel>();
            builder.Services.AddSingleton<OcrScanPage>();
            
            builder.Services.AddTransient<CircuitDetailViewModel>();
            
            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddSingleton<MainPage>();
            return builder.Build();
        }
    }
}