using NoteTakingApp.Models;

namespace NoteTakingApp.Services;

public class NoteService : INoteService
{
    public async Task<IEnumerable<Note>> LoadNotesAsync()
    {
        var notesList = new List<Note>();
        var appDataPath = Path.Combine(FileSystem.AppDataDirectory, "Notes");
        if (!Directory.Exists(appDataPath)) Directory.CreateDirectory(appDataPath);

        var noteFiles = Directory.GetFiles(appDataPath, "*.txt");

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

        return notesList.OrderByDescending(n => n.Date);
    }

    public Task DeleteNoteAsync(Note note)
    {
        if (note == null) return Task.CompletedTask;

        var filePath = Path.Combine(FileSystem.AppDataDirectory, "Notes", $"{note.Filename}.txt");
        if (File.Exists(filePath)) File.Delete(filePath);
        
        return Task.CompletedTask;
    }
}
