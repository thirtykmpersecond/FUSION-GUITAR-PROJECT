namespace FusionGuitar.Web.Theory;

/// <summary>A single note of a lick: sounding pitch plus optional fretboard placement.</summary>
public sealed record LickNote(
    int Midi,
    double Beats,
    int? StringIndex = null,
    int? Fret = null,
    string? Label = null);

/// <summary>A named improvisational phrase with a harmonic/scale context.</summary>
public sealed record Lick(
    string Name,
    string Style,
    string? Key = null,
    string? Backing = null,
    string? Description = null,
    IReadOnlyList<LickNote>? Notes = null);

public static class LickBuilder
{
    /// <summary>Build a lick from compact tokens: "60:1 62:0.5 64:0.5" (midi:beats).</summary>
    public static IReadOnlyList<LickNote> FromMidi(string spec)
    {
        var result = new List<LickNote>();
        foreach (var raw in spec.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var parts = raw.Split(':');
            if (parts.Length < 1 || !int.TryParse(parts[0], out var midi)) continue;
            var beats = 1.0;
            if (parts.Length > 1 && double.TryParse(parts[1], out var b)) beats = Math.Max(0.05, b);
            result.Add(new LickNote(midi, beats));
        }
        return result;
    }

    /// <summary>Build a lick from compact tokens: "5:3:1 4:0:0.5" (string:fret:beats).
    /// String numbering follows VexFlow (1 = high E); the stored StringIndex uses
    /// guitar convention (0 = low E) to match GuitarFretboard.</summary>
    public static IReadOnlyList<LickNote> FromFrets(string spec)
    {
        var openMidi = new[] { 40, 45, 50, 55, 59, 64 }; // index 0 = low E
        var result = new List<LickNote>();
        foreach (var raw in spec.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var parts = raw.Split(':');
            if (parts.Length < 2) continue;
            if (!int.TryParse(parts[0], out var str) || str < 1 || str > 6) continue;
            if (!int.TryParse(parts[1], out var fret)) continue;
            var beats = parts.Length > 2 && double.TryParse(parts[2], out var b) ? Math.Max(0.05, b) : 1.0;
            var guitarIndex = 6 - str; // 0 = low E
            result.Add(new LickNote(openMidi[guitarIndex] + fret, beats, guitarIndex, fret));
        }
        return result;
    }
}
