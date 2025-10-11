using System.ComponentModel;
using NoteTakingApp.Models;

namespace NoteTakingApp.Views;

public partial class NotePage : ContentPage, INotifyPropertyChanged
{
    private string _filename = string.Empty;
    public bool IsExistingNote => !string.IsNullOrEmpty(_filename);

    public NotePage()
    {
        InitializeComponent();
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        TitleEntry.Focus();
        
        // Get the filename from query parameters
        var route = Shell.Current.CurrentState.Location.ToString();
        var queryStart = route.IndexOf('?');
        if (queryStart > 0)
        {
            var query = route.Substring(queryStart + 1);
            var parameters = query.Split('&');
            
            foreach (var param in parameters)
            {
                var parts = param.Split('=');
                if (parts.Length == 2 && parts[0] == "filename")
                {
                    _filename = Uri.UnescapeDataString(parts[1]);
                    OnPropertyChanged(nameof(IsExistingNote));
                    
                    if (!string.IsNullOrEmpty(_filename))
                    {
                        await LoadExistingNote();
                    }
                    break;
                }
            }
        }
    }

    private async Task LoadExistingNote()
    {
        try
        {
            var appDataPath = Path.Combine(FileSystem.AppDataDirectory, "Notes");
            var filePath = Path.Combine(appDataPath, $"{_filename}.txt");

            if (File.Exists(filePath))
            {
                var content = await File.ReadAllTextAsync(filePath);
                TitleEntry.Text = _filename;
                NoteEditor.Text = content;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load note: {ex.Message}", "OK");
        }
    }
    
    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        try
        {
            var title = TitleEntry.Text?.Trim();
            var content = NoteEditor.Text?.Trim();

            if (string.IsNullOrEmpty(title))
            {
                await DisplayAlert("Error", "Please enter a title for your note.", "OK");
                return;
            }

            var appDataPath = Path.Combine(FileSystem.AppDataDirectory, "Notes");
            // sanitize filename to avoid invalid characters or path segments
            title = SanitizeFileName(title);
            
            // Create directory if it doesn't exist
            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }

            var filePath = Path.Combine(appDataPath, $"{title}.txt");

            // Write to a temp file first, then move to final name. This avoids data loss if write fails.
            var tempPath = Path.Combine(appDataPath, $"{Guid.NewGuid():N}.tmp");
            await File.WriteAllTextAsync(tempPath, content ?? string.Empty);

            // If we're editing an existing note and the title changed, move new file into place then delete old file
            if (!string.IsNullOrEmpty(_filename) && _filename != title)
            {
                var oldFilePath = Path.Combine(appDataPath, $"{_filename}.txt");

                // Replace or move temp to new filename
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                File.Move(tempPath, filePath);

                // Delete the old file if it exists and is different
                if (File.Exists(oldFilePath) && !string.Equals(oldFilePath, filePath, StringComparison.OrdinalIgnoreCase))
                {
                    try { File.Delete(oldFilePath); } catch { /* ignore deletion failures */ }
                }
            }
            else
            {
                // New note or same title: replace existing file atomically
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                File.Move(tempPath, filePath);
            }
            
            await DisplayAlert("Success", "Note saved successfully!", "OK");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to save note: {ex.Message}", "OK");
        }
    }

    public new event PropertyChangedEventHandler? PropertyChanged;
    protected virtual new void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private static string SanitizeFileName(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input ?? string.Empty;

        var invalid = Path.GetInvalidFileNameChars();
        var sb = new System.Text.StringBuilder(input.Length);
        foreach (var c in input)
        {
            if (invalid.Contains(c)) sb.Append('_'); else sb.Append(c);
        }
        return sb.ToString();
    }
}
