using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NoteTakingApp.Models;
using NoteTakingApp.Views;
using NoteTakingApp.Services;

namespace NoteTakingApp.ViewModels;

public partial class AllNotesViewModel : BaseViewModel
{
    private readonly IJokeService _jokeService;
    private readonly INoteService _noteService;
    private readonly ISerializationService _serializationService;
    [ObservableProperty]
    private string _jokeOfTheDay = "Loading joke...";
    [ObservableProperty]
    private DateTime _currentDate;
    private List<Note> _allNotes = new();
    public ObservableCollection<Note> Notes { get; } = new();
    public ObservableCollection<Note> IncomingTasks { get; } = new();


    [ObservableProperty]
    private string _searchQuery;

    partial void OnSearchQueryChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            Notes.Clear();
            foreach (var note in _allNotes)
            {
                Notes.Add(note);
            }
        }
        else
        {
            var filteredNotes = _allNotes.Where(n => n.Text.Contains(value, StringComparison.OrdinalIgnoreCase) || n.Filename.Contains(value, StringComparison.OrdinalIgnoreCase)).ToList();
            Notes.Clear();
            foreach (var note in filteredNotes)
            {
                Notes.Add(note);
            }
        }
    }

    [ObservableProperty]
    private bool _isEmpty;

    public AllNotesViewModel(IJokeService jokeService, INoteService noteService, ISerializationService serializationService)
    {
        _jokeService = jokeService;
        _noteService = noteService;
        _serializationService = serializationService;
        Notes.CollectionChanged += (s, e) => IsEmpty = Notes.Count == 0;
        CurrentDate = DateTime.Now;
        GetJokeCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadNotes()
    {
        try
        {
            Notes.Clear();
            var appDataPath = Path.Combine(FileSystem.AppDataDirectory, "Notes");
            if (!Directory.Exists(appDataPath)) Directory.CreateDirectory(appDataPath);

            var noteFiles = Directory.GetFiles(appDataPath, "*.txt");
            var notesList = new List<Note>();

            foreach (var file in noteFiles)
            {
                var fileInfo = new FileInfo(file);
                notesList.Add(new Note
                {
                    Filename = Path.GetFileNameWithoutExtension(file),
                    Text = await File.ReadAllTextAsync(file),
                    Date = fileInfo.LastWriteTime
                });
            }


            var sortedNotes = notesList.OrderByDescending(n => n.Date).ToList();
            _allNotes = sortedNotes;

            IncomingTasks.Clear();
            var tasksPath = Path.Combine(FileSystem.AppDataDirectory, "Tasks");
            if (Directory.Exists(tasksPath))
            {
                var taskFiles = Directory.GetFiles(tasksPath, "*.txt");
                foreach (var file in taskFiles)
                {
                    var fileInfo = new FileInfo(file);
                    var content = await File.ReadAllTextAsync(file);
                    
                    Note note;
                    try
                    {
                         note = _serializationService.Deserialize<Note>(content);
                         if (note == null) throw new Exception();
                    }
                    catch
                    {
                        note = new Note
                        {
                            Filename = Path.GetFileNameWithoutExtension(file),
                            Text = content,
                            Date = fileInfo.LastWriteTime
                        };
                    }

                    if (note.Date.Date >= DateTime.Today && note.Date.Date <= DateTime.Today.AddDays(7))
                    {
                        IncomingTasks.Add(note);
                    }
                }
            }

            OnSearchQueryChanged(SearchQuery);
        }
        catch (Exception ex)

        {
            await Shell.Current.DisplayAlert("Error", $"Failed to load notes: {ex.Message}", "OK");
        }
    }



    [RelayCommand]
    private async Task GetJoke()
    {
        JokeOfTheDay = "Finding a new joke...";
        try
        {
            JokeOfTheDay = await _jokeService.GetProgrammingJokeAsync();
        }
        catch (Exception ex)
        {
            JokeOfTheDay = $"Failed to load joke: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task GoToAddNote()
    {
        // Pass an empty filename to indicate a new note.
        await Shell.Current.GoToAsync($"{nameof(NotePage)}?Filename=");
    }

    [RelayCommand]
    private async Task EditNote(Note note)
    {
        if (note == null) return;
        await Shell.Current.GoToAsync($"{nameof(NotePage)}?Filename={Uri.EscapeDataString(note.Filename)}");
    }

    [RelayCommand]
    private async Task GoToNote(Note note)
    {
        if (note == null) return;
        await Shell.Current.GoToAsync($"{nameof(NoteDetailsPage)}?Filename={Uri.EscapeDataString(note.Filename)}");
    }
    [RelayCommand]
    private async Task GoToAllNotes()
    {
        await Shell.Current.GoToAsync($"{nameof(AllNotesPage)}");
    }

    [RelayCommand]
    private async Task GoToToDoList()
    {
        await Shell.Current.GoToAsync($"{nameof(ToDoListPage)}");
    }

    [RelayCommand]
    private async Task DeleteNote(Note note)
    {
        if (note == null) return;

        if (await Shell.Current.DisplayAlert("Delete Note", $"Delete '{note.Filename}'?", "Yes", "No"))
        {
            var filePath = Path.Combine(FileSystem.AppDataDirectory, "Notes", $"{note.Filename}.txt");
            if (File.Exists(filePath)) File.Delete(filePath);
            Notes.Remove(note);
        }
    }
}
