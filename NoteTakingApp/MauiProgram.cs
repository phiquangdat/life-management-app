using CommunityToolkit.Maui;
using NoteTakingApp.ViewModels;
using NoteTakingApp.Views;
using NoteTakingApp.Services;

namespace NoteTakingApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            // Initialize the .NET MAUI Community Toolkit, which is needed for MVVM features
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // --- Dependency Injection Setup ---
        builder.Services.AddHttpClient();

        builder.Services.AddSingleton<IJokeService, JokeService>();

        // Singleton services are created once for the entire app lifetime.
        // This is suitable for your main page and its ViewModel.
        builder.Services.AddSingleton<AllNotesViewModel>();
        builder.Services.AddSingleton<AllNotesPage>();

        // Transient services are created fresh every time they are needed.
        // This is essential for the note editing page, so you get a
        // clean, new instance each time you create or edit a note.
        builder.Services.AddTransient<NoteViewModel>();
        builder.Services.AddTransient<NotePage>();

        return builder.Build();
    }
}

