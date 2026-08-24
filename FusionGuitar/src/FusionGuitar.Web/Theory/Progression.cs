namespace FusionGuitar.Web.Theory;

/// <summary>A named chord progression: a sequence of chords with per-chord bar counts.</summary>
public sealed record Progression(
    string Name,
    string Key,
    IReadOnlyList<ProgressionStep> Steps,
    string? Description = null)
{
    public string Title => $"{Name}（{Key}）";
}

/// <summary>One step: a chord symbol and how many bars it lasts (default 1).</summary>
public sealed record ProgressionStep(string ChordSymbol, int Bars = 1, string? Feel = null);

public static class Progressions
{
    /// <summary>Parse a compact list like "Cmaj7,A7,Dm7,G7" or with bars "Cmaj7:2,A7:2".</summary>
    public static IReadOnlyList<ProgressionStep> ParseSteps(string chords, int defaultBars = 1)
    {
        var result = new List<ProgressionStep>();
        foreach (var raw in chords.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var parts = raw.Split(':', StringSplitOptions.TrimEntries);
            var symbol = parts[0].Trim();
            if (symbol.Length == 0) continue;
            int bars = defaultBars;
            string? feel = null;
            if (parts.Length > 1)
            {
                if (int.TryParse(parts[1], out var b)) bars = Math.Max(1, b);
                else feel = parts[1];
            }
            if (parts.Length > 2 && int.TryParse(parts[2], out var b2)) bars = Math.Max(1, b2);
            result.Add(new ProgressionStep(symbol, bars, feel));
        }
        return result;
    }

    public static readonly Progression IiV1Major = new(
        "ii–V–I", "C 大调",
        ParseSteps("Dm7,G7,Cmaj7"),
        "爵士语汇的基石：ii→V→I，三个和弦一个循环。");

    public static readonly Progression IiV1Minor = new(
        "ii–V–I 小调", "A 小调",
        ParseSteps("Am7b5,D7,Gm7"),
        "小调 ii–V–i：半减七→属七→小七。");

    public static readonly Progression AutumnLeaves = new(
        "Autumn Leaves", "G 大调",
        ParseSteps("Cmaj7,Bm7b5,E7,Am7,Dm7,G7,Cmaj7,C7", 1),
        "经典爵士标准曲：大调 ii–V 转小调 ii–V。");

    public static readonly Progression RhythmChanges = new(
        "Rhythm Changes", "B♭ 大调",
        ParseSteps("Bbmaj7,G7,Cm7,F7,Dm7,G7,Cm7,F7", 1),
        "I–VI–II–V 的循环，Charlie Parker 的最爱。");

    public static readonly Progression SoWhat = new(
        "So What", "D Dorian",
        ParseSteps("Dm7,Dm7,Em7,Em7", 2),
        "Miles Davis：两小节一个和弦的模态铺底。");

    public static readonly Progression MaidenVoyage = new(
        "Maiden Voyage", "F7 色彩",
        ParseSteps("Fm7,Bb7,Em7,Am7,Dm7", 2),
        "Herbie Hancock：每和弦两小节，Sus 色彩铺底。");

    public static readonly Progression CantaloupeIsland = new(
        "Cantaloupe Island", "Fm 蓝调",
        ParseSteps("Fm7,D7,Gm7,C7", 1),
        "经典 Funk 爵士 vamp，i–bVII–ii–V。");

    public static readonly Progression CJamBlues = new(
        "C Jam Blues", "C 蓝调",
        ParseSteps("C7,C7,C7,C7,F7,F7,C7,C7,G7,F7,C7,G7", 1),
        "最简单直接的 12 小节布鲁斯。");

    public static readonly IReadOnlyList<Progression> All = new[]
    {
        IiV1Major, IiV1Minor, AutumnLeaves, RhythmChanges,
        SoWhat, MaidenVoyage, CantaloupeIsland, CJamBlues
    };
}
