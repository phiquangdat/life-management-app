using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NoteTakingApp.Services;

namespace NoteTakingApp.ViewModels;

[QueryProperty(nameof(Filename), "Filename")]
public partial class AddTaskViewModel : BaseViewModel
{
    private readonly ISerializationService _serializationService;
    private string? _originalFilename;

    [ObservableProperty]
    private string? _filename;

    [ObservableProperty]
    private bool _isCompleted;

    [ObservableProperty]
    private string? _taskTitle;

    [ObservableProperty]
    private string? _taskDescription;

    [ObservableProperty]
    private DateTime _taskDate = DateTime.Today;

    [ObservableProperty]
    private DateTime _currentDate = DateTime.Today;

    public AddTaskViewModel(ISerializationService serializationService)
    {
        _serializationService = serializationService;
    }

    partial void OnFilenameChanged(string? value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            _originalFilename = value;
            LoadTask(value);
        }
    }

    private async void LoadTask(string filename)
    {
        try
        {
            var appDataPath = Path.Combine(FileSystem.AppDataDirectory, "Tasks");
            var filePath = Path.Combine(appDataPath, $"{filename}.txt");

            if (File.Exists(filePath))
            {
                var content = await File.ReadAllTextAsync(filePath);
                try
                {
                    var note = _serializationService.Deserialize<Models.Note>(content);
                    if (note != null)
                    {
                        TaskTitle = note.Filename;
                        TaskDescription = note.Text;
                        TaskDate = note.Date;
                        IsCompleted = note.IsCompleted;
                    }
                }
                catch
                {
                    // Fallback for legacy
                    TaskTitle = filename;
                    TaskDescription = content;
                    TaskDate = File.GetCreationTime(filePath);
                    IsCompleted = false;
                }
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to load task: {ex.Message}", "OK");
        }
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
            var newFilename = $"{validTitle}.txt";
            var newFilePath = Path.Combine(appDataPath, newFilename);

            // Check if renaming
            if (!string.IsNullOrEmpty(_originalFilename) && _originalFilename != validTitle)
            {
                 var oldFilePath = Path.Combine(appDataPath, $"{_originalFilename}.txt");
                 if (File.Exists(oldFilePath))
                 {
                     File.Delete(oldFilePath);
                 }
            }

            var note = new Models.Note
            {
                Filename = validTitle,
                Text = TaskDescription ?? string.Empty,
                Date = TaskDate,
                IsCompleted = IsCompleted
            };

            var json = _serializationService.Serialize(note);
            await File.WriteAllTextAsync(newFilePath, json);

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
