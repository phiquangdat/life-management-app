using Microsoft.Extensions.Logging;
using NoteTakingApp.Views;

namespace NoteTakingApp
{
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

            // Register pages for dependency injection
            builder.Services.AddSingleton<AllNotesPage>();
            builder.Services.AddTransient<NotePage>();

            return builder.Build();
        }
    }
}
