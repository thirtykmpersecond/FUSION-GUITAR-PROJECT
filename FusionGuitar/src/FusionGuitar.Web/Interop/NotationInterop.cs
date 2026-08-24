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
        bool autoBeam = true)
    {
        var m = await _module.Value;
        var dto = notes.Select(n => new
        {
            keys = n.Keys.ToArray(),
            duration = n.Duration
        }).ToArray();
        await m.InvokeVoidAsync("renderStave", elementId, new
        {
            width,
            clef,
            timeSig,
            keySig,
            notes = dto,
            autoBeam
        });
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

public sealed record NotationNote(IReadOnlyList<string> Keys, string Duration);
