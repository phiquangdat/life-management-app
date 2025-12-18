using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NoteTakingApp.Models;
using NoteTakingApp.Views;
using NoteTakingApp.Services;

namespace NoteTakingApp.ViewModels;

public partial class ToDoListViewModel : BaseViewModel
{
    private readonly ISerializationService _serializationService;
    private List<Note> _allNotes = new();
    public ObservableCollection<Note> Tasks { get; } = new();

    public ToDoListViewModel(ISerializationService serializationService)
    {
        _serializationService = serializationService;
    }

    [RelayCommand]
    private async Task LoadTasks()
    {
        try
        {
            Tasks.Clear();
            var appDataPath = Path.Combine(FileSystem.AppDataDirectory, "Tasks");
            if (!Directory.Exists(appDataPath)) Directory.CreateDirectory(appDataPath);

            var noteFiles = Directory.GetFiles(appDataPath, "*.txt");
            var notesList = new List<Note>();

            foreach (var file in noteFiles)
            {
                var fileInfo = new FileInfo(file);
                var content = await File.ReadAllTextAsync(file);
                
                Note note;
                try 
                {
                    // Try to deserialize as JSON
                    note = _serializationService.Deserialize<Note>(content);
                    // Ensure the date is set if not present in JSON
                    if (note == null) throw new Exception("Deserialization returned null");
                }
                catch
                {
                    // Fallback for legacy text files
                    note = new Note
                    {
                        Filename = Path.GetFileNameWithoutExtension(file),
                        Text = content,
                        Date = fileInfo.LastWriteTime
                    };
                }

                notesList.Add(note);
            }

            var sortedNotes = notesList.OrderBy(n => n.Date).ToList(); // Sort by date ascending for tasks
            _allNotes = sortedNotes;

            foreach (var note in _allNotes.Where(n => n.Date.Date >= DateTime.Today))
            {
                Tasks.Add(note);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to load tasks: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task AddTask()
    {
        await Shell.Current.GoToAsync($"{nameof(AddTaskPage)}");
    }

    [RelayCommand]
    private async Task GoToTaskDetails(Note note)
    {
        if (note == null) return;
        await Shell.Current.GoToAsync($"{nameof(NoteDetailsPage)}?Filename={Uri.EscapeDataString(note.Filename)}");
    }
}
