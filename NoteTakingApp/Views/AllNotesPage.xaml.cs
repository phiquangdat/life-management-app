using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
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
            // Clear existing notes
            Notes.Clear();
            OnPropertyChanged(nameof(IsEmpty));
            
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
            
            // Create a list to hold notes temporarily
            var notesList = new List<Note>();
            
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
                
                notesList.Add(note);
            }

            // Sort by date (newest first)
            var sortedNotes = notesList.OrderByDescending(n => n.Date).ToList();
            
            // Add sorted notes to the observable collection
            foreach (var note in sortedNotes)
            {
                Notes.Add(note);
            }

            // Trigger property change notification for UI update
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

    private async void OnEditNoteClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Note selectedNote)
        {
            // Navigate to note editing page with selected note
            await Shell.Current.GoToAsync($"{nameof(NotePage)}?filename={Uri.EscapeDataString(selectedNote.Filename)}");
        }
    }

    private async void OnDeleteNoteClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Note selectedNote)
        {
            // Show confirmation dialog
            var result = await DisplayAlert(
                "Delete Note", 
                $"Are you sure you want to delete '{selectedNote.Filename}'? This action cannot be undone.", 
                "Delete", 
                "Cancel");

            if (result)
            {
                try
                {
                    var appDataPath = Path.Combine(FileSystem.AppDataDirectory, "Notes");
                    var filePath = Path.Combine(appDataPath, $"{selectedNote.Filename}.txt");

                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }

                    // Remove from collection and refresh
                    Notes.Remove(selectedNote);
                    OnPropertyChanged(nameof(IsEmpty));
                    
                    await DisplayAlert("Success", "Note deleted successfully!", "OK");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Failed to delete note: {ex.Message}", "OK");
                }
            }
        }
    }

    public new event PropertyChangedEventHandler? PropertyChanged;
    protected virtual new void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

// Text preview converter to show truncated text
public class TextPreviewConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string text)
        {
            // Remove newlines and limit length for preview
            var cleanText = text.Replace('\n', ' ').Replace('\r', ' ').Trim();
            return cleanText.Length > 100 ? cleanText.Substring(0, 100) + "..." : cleanText;
        }
        return string.Empty;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
