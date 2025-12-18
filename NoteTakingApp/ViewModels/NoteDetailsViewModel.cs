using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NoteTakingApp.Models;

namespace NoteTakingApp.ViewModels;

[QueryProperty(nameof(Filename), "Filename")]
public partial class NoteDetailsViewModel : BaseViewModel
{
    [ObservableProperty]
    private string? _filename;

    [ObservableProperty]
    private string? _text;

    [ObservableProperty]
    private string? _title;

    [ObservableProperty]
    private DateTime _date;

    public NoteDetailsViewModel() { }

    partial void OnFilenameChanged(string? value)
    {
        LoadNote(value);
    }

    private async void LoadNote(string? filename)
    {
        try
        {
            if (!string.IsNullOrEmpty(filename))
            {
                var filePath = Path.Combine(FileSystem.AppDataDirectory, "Notes", $"{filename}.txt");
                if (File.Exists(filePath))
                {
                    Title = filename;
                    Text = await File.ReadAllTextAsync(filePath);
                    Date = File.GetLastWriteTime(filePath);
                }
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to load note: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task EditNote()
    {
        if (string.IsNullOrEmpty(Filename)) return;
        await Shell.Current.GoToAsync($"{nameof(Views.NotePage)}?Filename={Uri.EscapeDataString(Filename)}");
    }

    [RelayCommand]
    private async Task GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }
}
