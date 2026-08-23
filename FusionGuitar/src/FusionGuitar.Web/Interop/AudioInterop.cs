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

    public async ValueTask DisposeAsync()
    {
        if (_module.IsValueCreated)
        {
            var m = await _module.Value;
            await m.DisposeAsync();
        }
    }
}
