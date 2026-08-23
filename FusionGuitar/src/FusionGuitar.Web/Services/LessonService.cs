using System.Net.Http.Json;

namespace FusionGuitar.Web.Services;

public sealed class LessonManifest
{
    public List<LessonManifestModule> Modules { get; set; } = new();
}

public sealed class LessonManifestModule
{
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public int Order { get; set; }
    public List<LessonManifestEntry> Lessons { get; set; } = new();
}

public sealed class LessonManifestEntry
{
    public string Slug { get; set; } = "";
    public string Title { get; set; } = "";
    public string Summary { get; set; } = "";
    public int Order { get; set; }
    public List<string> Tags { get; set; } = new();
}

public sealed class LessonService(HttpClient http)
{
    private const string ManifestPath = "lessons/index.json";
    private LessonManifest? _manifest;
    private readonly Dictionary<string, Lesson> _cache = new();

    public async Task<LessonManifest> GetManifestAsync(CancellationToken ct = default)
    {
        if (_manifest is not null) return _manifest;
        _manifest = await http.GetFromJsonAsync<LessonManifest>(ManifestPath, ct)
                    ?? new LessonManifest();
        return _manifest;
    }

    public async Task<IReadOnlyList<LessonNavItem>> GetNavAsync(CancellationToken ct = default)
    {
        var m = await GetManifestAsync(ct);
        return m.Modules
            .OrderBy(mod => mod.Order)
            .SelectMany(mod => mod.Lessons
                .OrderBy(l => l.Order)
                .Select(l => new LessonNavItem(l.Slug, l.Title, mod.Id, mod.Title, l.Order)))
            .ToList();
    }

    public async Task<Lesson?> GetAsync(string moduleId, string slug, CancellationToken ct = default)
    {
        var key = $"{moduleId}/{slug}";
        if (_cache.TryGetValue(key, out var cached)) return cached;

        var manifest = await GetManifestAsync(ct);
        var mod = manifest.Modules.FirstOrDefault(m => m.Id == moduleId);
        var entry = mod?.Lessons.FirstOrDefault(l => l.Slug == slug);
        if (mod is null || entry is null) return null;

        var path = $"lessons/{moduleId}/{slug}.md";
        var md = await http.GetStringAsync(path, ct);
        var meta = new LessonMeta(mod.Id, entry.Slug, entry.Title, entry.Summary, entry.Order, entry.Tags);
        var lesson = new Lesson(meta, md);
        _cache[key] = lesson;
        return lesson;
    }

    public async Task<(LessonNavItem? Prev, LessonNavItem? Next)> GetNeighborsAsync(
        string slug, CancellationToken ct = default)
    {
        var nav = await GetNavAsync(ct);
        var idx = nav.ToList().FindIndex(n => n.Slug == slug);
        if (idx < 0) return (null, null);
        var prev = idx > 0 ? nav[idx - 1] : null;
        var next = idx < nav.Count - 1 ? nav[idx + 1] : null;
        return (prev, next);
    }
}
