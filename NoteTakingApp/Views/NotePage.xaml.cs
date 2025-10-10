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
            
            // Create directory if it doesn't exist
            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }

            var filePath = Path.Combine(appDataPath, $"{title}.txt");
            
            // If we're editing an existing note and the title changed, delete the old file
            if (!string.IsNullOrEmpty(_filename) && _filename != title)
            {
                var oldFilePath = Path.Combine(appDataPath, $"{_filename}.txt");
                if (File.Exists(oldFilePath))
                {
                    File.Delete(oldFilePath);
                }
            }

            await File.WriteAllTextAsync(filePath, content ?? string.Empty);
            
            await DisplayAlert("Success", "Note saved successfully!", "OK");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to save note: {ex.Message}", "OK");
        }
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_filename))
            return;

        var result = await DisplayAlert(
            "Delete Note", 
            $"Are you sure you want to delete '{_filename}'?", 
            "Delete", 
            "Cancel");

        if (result)
        {
            try
            {
                var appDataPath = Path.Combine(FileSystem.AppDataDirectory, "Notes");
                var filePath = Path.Combine(appDataPath, $"{_filename}.txt");
                
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    await DisplayAlert("Success", "Note deleted successfully!", "OK");
                    await Shell.Current.GoToAsync("..");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to delete note: {ex.Message}", "OK");
            }
        }
    }

    public new event PropertyChangedEventHandler? PropertyChanged;
    protected virtual new void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
