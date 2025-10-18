using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using NoteTakingApp.Models;
using NoteTakingApp.Views;

namespace NoteTakingApp.ViewModels;

public partial class AllNotesViewModel : BaseViewModel
{
  public ObservableCollection<Note> Notes { get; } = new();

  public AllNotesViewModel()
  {
    LoadNotes();
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

      var sortedNotes = notesList.OrderByDescending(n => n.Date);
      foreach (var note in sortedNotes)
      {
        Notes.Add(note);
      }
    }
    catch (Exception ex)
    {
      await Shell.Current.DisplayAlert("Error", $"Failed to load notes: {ex.Message}", "OK");
    }
  }

  private async Task GoToAddNote()
  {
    await Shell.Current.GoToAsync(nameof(NotePage));
  }
  private async Task GoToNote(Note note)
  {
    await Shell.Current.GoToAsync(nameof(NotePage));
  }
    private async Task DeleteNote(Note note)
  {
    if (note == null) return;
    var result = await Shell.Current.DisplayAlert("Delete Note", $"Delete, '{note.Filename}'?", "Yes", "No");
    if(result)
        {
      Notes.Remove(note);
      var filePath = Path.Combine(FileSystem.AppDataDirectory, "Notes", $"{note.Filename}.txt");
      if (File.Exists(filePath))
            {
        File.Delete(filePath);
            }
        }
    }
}
