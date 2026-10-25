using HeimdallTemplateApp.Models;

namespace HeimdallTemplateApp.Services;

public sealed class NoteStore
{
    private readonly object _gate = new();
    private readonly List<NoteEntity> _notes = [];

    public IReadOnlyList<NoteEntity> GetAll()
    {
        lock (_gate)
        {
            return _notes.OrderByDescending(x => x.CreatedUtc).ToArray();
        }
    }

    public void Add(CreateNoteRequest request)
    {
        lock (_gate)
        {
            _notes.Add(new NoteEntity { Title = request.Title, Content = request.Content });
        }
    }
}
