using System.Collections.ObjectModel;
using System.ComponentModel;
using NoteTakingApp.Models;

namespace NoteTakingApp.Views;

public partial class AllNotesPage : ContentPage, INotifyPropertyChanged
{
    public ObservableCollection<Note> Notes { get; set; } = new();
    public bool IsEmpty => Notes.Count == 0;

    public AllNotesPage()
    {
        InitializeComponent();
        BindingContext = this;
        LoadNotes();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadNotes();
    }

    private async void LoadNotes()
    {
        try
        {
            Notes.Clear();
            
            // Get the app's data directory
            var appDataPath = Path.Combine(FileSystem.AppDataDirectory, "Notes");
            
            // Create directory if it doesn't exist
            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
                return;
            }

            // Load all note files
            var noteFiles = Directory.GetFiles(appDataPath, "*.txt");
            
            foreach (var file in noteFiles)
            {
                var filename = Path.GetFileNameWithoutExtension(file);
                var fileInfo = new FileInfo(file);
                
                var note = new Note
                {
                    Filename = filename,
                    Text = await File.ReadAllTextAsync(file),
                    // Use LastWriteTime so edits update ordering
                    Date = fileInfo.LastWriteTime
                };
                
                Notes.Add(note);
            }

            // Sort by date (newest first)
            var sortedNotes = Notes.OrderByDescending(n => n.Date).ToList();
            Notes.Clear();
            foreach (var note in sortedNotes)
            {
                Notes.Add(note);
            }

            OnPropertyChanged(nameof(IsEmpty));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load notes: {ex.Message}", "OK");
        }
    }

    private async void OnAddNoteClicked(object sender, EventArgs e)
    {
        // Navigate to note editing page with empty note
        await Shell.Current.GoToAsync($"{nameof(NotePage)}?filename=");
    }

    private async void OnNoteSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Note selectedNote)
        {
            // Clear selection
            NotesCollection.SelectedItem = null;
            
            // Navigate to note editing page with selected note
            await Shell.Current.GoToAsync($"{nameof(NotePage)}?filename={selectedNote.Filename}");
        }
    }

    public new event PropertyChangedEventHandler? PropertyChanged;
    protected virtual new void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
