using NoteTakingApp.Models;

namespace NoteTakingApp.Services;

public interface INoteService
{
    Task<IEnumerable<Note>> LoadNotesAsync();
    Task DeleteNoteAsync(Note note);
}
