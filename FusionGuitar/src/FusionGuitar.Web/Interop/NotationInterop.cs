using Microsoft.JSInterop;

namespace FusionGuitar.Web.Interop;

public sealed class NotationInterop(IJSRuntime js) : IAsyncDisposable
{
    private Lazy<Task<IJSObjectReference>> _module = new(() =>
        js.InvokeAsync<IJSObjectReference>("import", "./js/notation.js").AsTask());

    public async ValueTask RenderStaveAsync(
        string elementId,
        int width,
        string clef,
        string? timeSig,
        string? keySig,
        IReadOnlyList<NotationNote> notes,
        IReadOnlyList<TabNote>? tabNotes = null,
        DotNetObjectReference<NotationEvents>? events = null)
    {
        var m = await _module.Value;
        var dto = notes.Select(n => new
        {
            keys = n.Keys.ToArray(),
            duration = n.Duration
        }).ToArray();
        var tabDto = tabNotes?.Select(n => new
        {
            positions = n.Positions.Select(p => new { str = p.String, fret = p.Fret }).ToArray(),
            duration = n.Duration
        }).ToArray();
        await m.InvokeVoidAsync("renderStave", elementId, new
        {
            width,
            clef,
            timeSig,
            keySig,
            notes = dto,
            tabNotes = tabDto,
            onNoteClick = events
        });
    }

    public async ValueTask HighlightNoteAsync(string elementId, int index, string color = "#ef4444")
    {
        var m = await _module.Value;
        await m.InvokeVoidAsync("highlightNote", elementId, index, color);
    }

    public async ValueTask ClearHighlightAsync(string elementId)
    {
        var m = await _module.Value;
        await m.InvokeVoidAsync("clearHighlight", elementId);
    }

    public async ValueTask DisposeAsync()
    {
        if (_module.IsValueCreated)
        {
            var m = await _module.Value;
            await m.DisposeAsync();
        }
    }
}

/// <summary>Receives note-click callbacks from the rendered notation.</summary>
public sealed class NotationEvents
{
    public event Action<int>? NoteClicked;

    [JSInvokable]
    public void OnNoteClick(int index) => NoteClicked?.Invoke(index);
}

public sealed record NotationNote(IReadOnlyList<string> Keys, string Duration);
public sealed record TabPosition(int String, int Fret);
public sealed record TabNote(IReadOnlyList<TabPosition> Positions, string Duration);
