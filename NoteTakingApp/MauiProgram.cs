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
        builder.Services.AddSingleton<INoteService, NoteService>();
        builder.Services.AddSingleton<ISerializationService, SerializationService>();

        // Singleton services are created once for the entire app lifetime.
        // This is suitable for your main page and its ViewModel.
        builder.Services.AddSingleton<MainPage>();

        // Transient services are created fresh every time they are needed.
        // This is essential for the note editing page, so you get a
        // clean, new instance each time you create or edit a note.
        builder.Services.AddTransient<AllNotesViewModel>();
        builder.Services.AddTransient<NoteViewModel>();
        builder.Services.AddTransient<NotePage>();
        builder.Services.AddTransient<AllNotesPage>();
        builder.Services.AddTransient<NoteDetailsViewModel>();
        builder.Services.AddTransient<NoteDetailsPage>();
        builder.Services.AddTransient<ToDoListViewModel>();
        builder.Services.AddTransient<ToDoListPage>();
        builder.Services.AddTransient<AddTaskViewModel>();
        builder.Services.AddTransient<AddTaskPage>();
        builder.Services.AddTransient<TaskDetailsViewModel>();
        builder.Services.AddTransient<TaskDetailsPage>();

        return builder.Build();
    }
}

