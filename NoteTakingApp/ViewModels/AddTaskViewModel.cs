using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NoteTakingApp.Services;

namespace NoteTakingApp.ViewModels;

public partial class AddTaskViewModel : BaseViewModel
{
    private readonly ISerializationService _serializationService;

    [ObservableProperty]
    private string _taskTitle;

    [ObservableProperty]
    private string _taskDescription;

    [ObservableProperty]
    private DateTime _taskDate = DateTime.Today;

    [ObservableProperty]
    private DateTime _currentDate = DateTime.Today;

    public AddTaskViewModel(ISerializationService serializationService)
    {
        _serializationService = serializationService;
    }

    [RelayCommand]
    private async Task SaveTask()
    {
        if (string.IsNullOrWhiteSpace(TaskTitle))
        {
            await Shell.Current.DisplayAlert("Error", "Please enter a task name.", "OK");
            return;
        }

        try
        {
            var appDataPath = Path.Combine(FileSystem.AppDataDirectory, "Tasks");
            if (!Directory.Exists(appDataPath)) Directory.CreateDirectory(appDataPath);

            var invalidChars = Path.GetInvalidFileNameChars();
            var validTitle = new string(TaskTitle.Where(ch => !invalidChars.Contains(ch)).ToArray());
            var filename = $"{validTitle}.txt";

            var filePath = Path.Combine(appDataPath, filename);

            var note = new Models.Note
            {
                Filename = validTitle,
                Text = TaskDescription ?? string.Empty,
                Date = TaskDate
            };

            var json = _serializationService.Serialize(note);
            await File.WriteAllTextAsync(filePath, json);

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to save task: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task Cancel()
    {
        await Shell.Current.GoToAsync("..");
    }
}
