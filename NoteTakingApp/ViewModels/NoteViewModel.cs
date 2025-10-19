using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NoteTakingApp.Models;

namespace NoteTakingApp.ViewModels;

[QueryProperty(nameof(Filename), "Filename")]
public partial class NoteViewModel : BaseViewModel
{
    [ObservableProperty]
    private string? _filename;

    [ObservableProperty]
    private string? _text;

    [ObservableProperty]
    private string? _title;

    public NoteViewModel() {}

    partial void OnFilenameChanged(string? value)
    {
        LoadNote(value);
    }

    private async void LoadNote(string? filename)
    {
        try
        {
            if (string.IsNullOrEmpty(filename))
            {
                Title = string.Empty;
                Text = string.Empty;
            }
            else
            {
                var filePath = Path.Combine(FileSystem.AppDataDirectory, "Notes", $"{filename}.txt");
                if (File.Exists(filePath))
                {
                    Title = filename;
                    Text = await File.ReadAllTextAsync(filePath);
                }
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to load note: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task SaveNote()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(Title))
            {
                await Shell.Current.DisplayAlert("Error", "Please enter a title for your note.", "OK");
                return;
            }

            var appDataPath = Path.Combine(FileSystem.AppDataDirectory, "Notes");
            if (!Directory.Exists(appDataPath)) Directory.CreateDirectory(appDataPath);

            var filePath = Path.Combine(appDataPath, $"{Title}.txt");
            await File.WriteAllTextAsync(filePath, Text ?? string.Empty);

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to save note: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task Cancel()
    {
        await Shell.Current.GoToAsync("..");
    }
}
