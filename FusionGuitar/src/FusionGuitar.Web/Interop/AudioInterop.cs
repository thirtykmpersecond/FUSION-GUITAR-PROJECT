using Microsoft.JSInterop;

namespace FusionGuitar.Web.Interop;

public sealed class AudioInterop(IJSRuntime js) : IAsyncDisposable
{
    private Lazy<Task<IJSObjectReference>> _module = new(() =>
        js.InvokeAsync<IJSObjectReference>("import", "./js/interop.js").AsTask());

    public async ValueTask InitAsync()
    {
        var m = await _module.Value;
        await m.InvokeVoidAsync("init");
    }

    public async ValueTask PlayNoteAsync(string noteWithOctave, double duration = 0.6, double velocity = 0.8)
    {
        var m = await _module.Value;
        await m.InvokeVoidAsync("playNote", noteWithOctave, duration, velocity);
    }

    public async ValueTask PlayMidiAsync(int midi, double duration = 0.6, double velocity = 0.8)
    {
        var m = await _module.Value;
        await m.InvokeVoidAsync("playMidi", midi, duration, velocity);
    }

    public async ValueTask PlayChordAsync(IEnumerable<string> notes, double duration = 1.2, double velocity = 0.7)
    {
        var m = await _module.Value;
        await m.InvokeVoidAsync("playChord", notes.ToArray(), duration, velocity);
    }

    public async ValueTask SetBpmAsync(int bpm)
    {
        var m = await _module.Value;
        await m.InvokeVoidAsync("setBpm", bpm);
    }

    public async ValueTask StartMetronomeAsync(int bpm)
    {
        var m = await _module.Value;
        await m.InvokeVoidAsync("startMetronome", bpm);
    }

    public async ValueTask StopMetronomeAsync()
    {
        var m = await _module.Value;
        await m.InvokeVoidAsync("stopMetronome");
    }

    public async ValueTask PlayProgressionAsync(IReadOnlyList<ProgressionChord> chords, int bpm)
    {
        var m = await _module.Value;
        var dto = chords.Select(c => new { notes = c.Notes.ToArray(), dur = c.Duration }).ToArray();
        await m.InvokeVoidAsync("playProgression", dto, bpm);
    }

    public async ValueTask StopProgressionAsync()
    {
        var m = await _module.Value;
        await m.InvokeVoidAsync("stopProgression");
    }

    public async ValueTask<int> PlaySequenceAsync(
        IReadOnlyList<LickNoteDto> notes,
        int bpm,
        DotNetObjectReference<SequenceEvents>? onNote = null)
    {
        var m = await _module.Value;
        var dto = notes.Select(n => new { midi = n.Midi, beats = n.Beats }).ToArray();
        return await m.InvokeAsync<int>("scheduleSequence", dto, bpm, onNote);
    }

    public async ValueTask StopSequenceAsync()
    {
        var m = await _module.Value;
        await m.InvokeVoidAsync("stopSequence");
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

public sealed record LickNoteDto(int Midi, double Beats);

/// <summary>Receives per-note playback callbacks (for synced highlighting).</summary>
public sealed class SequenceEvents
{
    public event Action<int>? NoteStarted;

    [JSInvokable]
    public void OnNote(int index) => NoteStarted?.Invoke(index);
}

public sealed record ProgressionChord(IReadOnlyList<string> Notes, string Duration);
