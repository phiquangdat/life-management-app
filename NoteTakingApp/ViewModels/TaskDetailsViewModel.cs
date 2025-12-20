using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NoteTakingApp.Models;

namespace NoteTakingApp.ViewModels;

[QueryProperty(nameof(Filename), "Filename")]
public partial class TaskDetailsViewModel : BaseViewModel
{
    private readonly Services.ISerializationService _serializationService;

    [ObservableProperty]
    private string? filename;

    [ObservableProperty]
    private string? text;

    [ObservableProperty]
    private string? title;

    [ObservableProperty]
    private DateTime date;

    [ObservableProperty]
    private bool isCompleted;

    public TaskDetailsViewModel(Services.ISerializationService serializationService)
    {
        _serializationService = serializationService;
    }

    partial void OnFilenameChanged(string? value)
    {
        if (!string.IsNullOrEmpty(value))
            _ = LoadTaskAsync(value);
    }

    private async Task LoadTaskAsync(string filename)
    {
        try
        {
            var tasksPath = Path.Combine(
                FileSystem.AppDataDirectory,
                "Tasks",
                $"{filename}.txt");

            if (!File.Exists(tasksPath))
                return;

            var content = await File.ReadAllTextAsync(tasksPath);
            var note = _serializationService.Deserialize<Note>(content);

            if (note != null)
            {
                Title = note.Filename;
                Text = note.Text;
                Date = note.Date;
                IsCompleted = note.IsCompleted;
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Error",
                $"Failed to load task: {ex.Message}",
                "OK");
        }
    }

    [RelayCommand]
    private async Task ToggleCompletion()
    {
        if (string.IsNullOrEmpty(Filename))
            return;

        try
        {
            IsCompleted = !IsCompleted;

            var tasksPath = Path.Combine(
                FileSystem.AppDataDirectory,
                "Tasks",
                $"{Filename}.txt");

            if (File.Exists(tasksPath))
            {
                var note = new Note
                {
                    Filename = Filename,
                    Text = Text ?? string.Empty,
                    Date = Date,
                    IsCompleted = IsCompleted
                };

                var json = _serializationService.Serialize(note);
                await File.WriteAllTextAsync(tasksPath, json);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Error",
                $"Failed to update task: {ex.Message}",
                "OK");
        }
    }

    [RelayCommand]
    private async Task EditTask()
    {
        if (string.IsNullOrEmpty(Filename))
            return;

        await Shell.Current.GoToAsync(
            $"{nameof(Views.AddTaskPage)}?Filename={Uri.EscapeDataString(Filename)}");
    }

    [RelayCommand]
    private async Task GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }
}