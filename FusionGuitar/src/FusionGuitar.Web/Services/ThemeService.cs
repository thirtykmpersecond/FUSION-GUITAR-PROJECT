using Microsoft.JSInterop;

namespace FusionGuitar.Web.Services;

/// <summary>Light/dark theme toggle with localStorage persistence.</summary>
public sealed class ThemeService(IJSRuntime js)
{
    private Lazy<Task<IJSObjectReference>> _module = new(() =>
        js.InvokeAsync<IJSObjectReference>("import", "./js/theme.js").AsTask());

    public async ValueTask<string> CurrentAsync()
    {
        var m = await _module.Value;
        return await m.InvokeAsync<string>("current");
    }

    public async ValueTask<string> ToggleAsync()
    {
        var m = await _module.Value;
        return await m.InvokeAsync<string>("toggle");
    }
}
