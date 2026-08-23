using Microsoft.JSInterop;

namespace FusionGuitar.Web.Services;

public sealed class ProgressService(IJSRuntime js)
{
    private const string StorageKey = "fg.progress.completed";
    private HashSet<string>? _completed;

    public event Action? Changed;

    public async Task<HashSet<string>> GetCompletedAsync()
    {
        if (_completed is not null) return _completed;
        try
        {
            var raw = await js.InvokeAsync<string?>("localStorage.getItem", StorageKey);
            _completed = string.IsNullOrEmpty(raw)
                ? new HashSet<string>()
                : raw.Split(',', StringSplitOptions.RemoveEmptyEntries).ToHashSet();
        }
        catch
        {
            _completed = new HashSet<string>();
        }
        return _completed;
    }

    public async Task<bool> IsCompletedAsync(string slug)
    {
        var set = await GetCompletedAsync();
        return set.Contains(slug);
    }

    public async Task SetCompletedAsync(string slug, bool completed)
    {
        var set = await GetCompletedAsync();
        if (completed) set.Add(slug); else set.Remove(slug);
        await js.InvokeVoidAsync("localStorage.setItem", StorageKey, string.Join(',', set));
        Changed?.Invoke();
    }

    public async Task ToggleAsync(string slug)
    {
        var set = await GetCompletedAsync();
        var done = set.Contains(slug);
        await SetCompletedAsync(slug, !done);
    }
}
